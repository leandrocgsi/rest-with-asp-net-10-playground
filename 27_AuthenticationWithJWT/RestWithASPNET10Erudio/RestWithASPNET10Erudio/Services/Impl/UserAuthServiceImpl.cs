using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Model;
using RestWithASPNET10Erudio.Repositories;

namespace RestWithASPNET10Erudio.Services.Impl
{
    public class UserAuthServiceImpl : IUserAuthService
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher _passwordHasher;

        public UserAuthServiceImpl(IUserRepository repository, IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public User? GetByUsername(string username)
        {
            return _repository.GetByUsername(username);
        }

        public User Create(AccountCredentialsDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entity = new User
            {
                UserName = dto.Username,
                FullName = dto.FullName,
                Password = _passwordHasher.Hash(dto.Password),
                RefreshToken = string.Empty,
                RefreshTokenExpiryTime = null
            };

            return _repository.Create(entity);
        }

        public bool RevokeToken(string username)
        {
            var user = _repository.GetByUsername(username);
            if (user == null) return false;

            user.RefreshToken = null;
            _repository.Update(user);
            return true;
        }

        public User Update(User user)
        {
            return _repository.Update(user);
        }
    }
}