using Microsoft.IdentityModel.JsonWebTokens;
using RestWithASPNET10Erudio.Auth;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Model;
using RestWithASPNETErudio.Data.DTO;
using RestWithASPNETErudio.Services;
using System.Security.Claims;

namespace RestWithASPNET10Erudio.Services.Impl
{
    public class LoginServiceImpl : ILoginService
    {
        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

        private readonly TokenConfiguration _configurations;
        private readonly ITokenService _tokenService;
        private readonly IUserAuthService _userAuthService;
        private readonly IPasswordHasher _passwordHasher;

        public LoginServiceImpl(
            TokenConfiguration configurations,
            ITokenService tokenService,
            IUserAuthService userAuthService,
            IPasswordHasher passwordHasher)
        {
            _configurations = configurations;
            _tokenService = tokenService;
            _userAuthService = userAuthService;
            _passwordHasher = passwordHasher;
        }

        public TokenDTO? ValidateCredentials(UserDTO userDto)
        {
            var user = _userAuthService.GetByUsername(userDto.Username);
            if (user == null) return null;

            if (!_passwordHasher.Verify(userDto.Password, user.Password))
                return null;

            return GenerateTokens(user);
        }

        public TokenDTO? ValidateCredentials(TokenDTO token)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(token.AccessToken);
            var username = principal.Identity?.Name;

            var user = _userAuthService.GetByUsername(username);
            if (user == null ||
                user.RefreshToken != token.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.Now)
                return null;

            return GenerateTokens(user, principal.Claims);
        }

        public bool RevokeToken(string username)
        {
            return _userAuthService.RevokeToken(username);
        }

        public AccountCredentialsDTO Create(AccountCredentialsDTO dto)
        {
            var user = _userAuthService.Create(dto);

            return new AccountCredentialsDTO
            {
                Username = user.UserName,
                FullName = user.FullName,
                Password = "*****************"
            };
        }

        private TokenDTO GenerateTokens(User user, IEnumerable<Claim>? existingClaims = null)
        {
            var claims = existingClaims?.ToList() ?? new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_configurations.DaysToExpiry);

            _userAuthService.Update(user);

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
    }
}