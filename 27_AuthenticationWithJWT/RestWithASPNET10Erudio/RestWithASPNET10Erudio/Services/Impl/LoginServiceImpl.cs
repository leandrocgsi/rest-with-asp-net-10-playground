using Microsoft.IdentityModel.JsonWebTokens;
using RestWithASPNET10Erudio.Auth;
using RestWithASPNETErudio.Data.DTO;
using RestWithASPNETErudio.Repository;
using RestWithASPNETErudio.Services;
using System.Security.Claims;

namespace RestWithASPNET10Erudio.Services.Impl
{
    public class LoginServiceImpl : ILoginService
    {
        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

        private readonly TokenConfiguration _configurations;
        private readonly IUserRepository _repository;
        private readonly ITokenService _tokenService;

        public LoginServiceImpl(IUserRepository repository, ITokenService tokenService, TokenConfiguration configurations)
        {
            _repository = repository;
            _tokenService = tokenService;
            _configurations = configurations;
        }

        public TokenDTO ValidateCredentials(UserDTO userDto)
        {
            var user = _repository.ValidateCredentials(userDto.Username, userDto.Password);
            if (user == null) return null;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_configurations.DaysToExpiry);

            _repository.RefreshUserInfo(user);

            var createDate = DateTime.Now;
            var expirationDate = createDate.AddMinutes(_configurations.Minutes);

            return new TokenDTO(
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
            );
        }

        public TokenDTO ValidateCredentials(TokenDTO token)
        {
            var accessToken = token.AccessToken;
            var refreshToken = token.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity?.Name;

            var user = _repository.ValidateCredentials(username);

            if (user == null ||
                user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.Now) return null;

            accessToken = _tokenService.GenerateAccessToken(principal.Claims);
            refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            _repository.RefreshUserInfo(user);

            var createDate = DateTime.Now;
            var expirationDate = createDate.AddMinutes(_configurations.Minutes);

            return new TokenDTO(
                true,
                createDate.ToString(DATE_FORMAT),
                expirationDate.ToString(DATE_FORMAT),
                accessToken,
                refreshToken
            );
        }

        public bool RevokeToken(string username)
        {
            return _repository.RevokeToken(username);
        }
    }
}
