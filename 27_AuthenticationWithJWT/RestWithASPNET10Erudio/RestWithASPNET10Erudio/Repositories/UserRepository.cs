using RestWithASPNET10Erudio.Model;
using RestWithASPNET10Erudio.Model.Context;
using RestWithASPNETErudio.Repository;
using System.Security.Cryptography;
using System.Text;

namespace RestWithASPNET10Erudio.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MSSQLContext _context;

        public UserRepository(MSSQLContext context)
        {
            _context = context;
        }

        public User ValidateCredentials(string username, string password)
        {
            var pass = ComputeHash(password, SHA256.Create());
            return _context.Users.FirstOrDefault(u =>
                u.UserName == username &&
                u.Password == pass);
        }

        public User ValidateCredentials(string username)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == username);
        }

        public User RefreshUserInfo(User user)
        {
            if (!_context.Users.Any(u => u.Id == user.Id)) return null;

            var result = _context.Users.SingleOrDefault(p => p.Id == user.Id);
            if (result != null)
            {
                try
                {
                    _context.Entry(result).CurrentValues.SetValues(user);
                    _context.SaveChanges();
                    return result;
                }
                catch
                {
                    throw;
                }
            }
            return result;
        }

        public bool RevokeToken(string username)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserName == username);
            if (user is null) return false;

            user.RefreshToken = null;
            _context.SaveChanges();
            return true;
        }

        private string ComputeHash(string input, HashAlgorithm algorithm)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

            var builder = new StringBuilder();
            foreach (var item in hashedBytes)
            {
                builder.Append(item.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
