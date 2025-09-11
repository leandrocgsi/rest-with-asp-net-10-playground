using RestWithASPNET10Erudio.Model;
using RestWithASPNET10Erudio.Model.Context;
using RestWithASPNETErudio.Repository;
using System.Security.Cryptography;
using System.Text;

namespace RestWithASPNET10Erudio.Repositories
{
    public class UserRepository(MSSQLContext context) : IUserRepository
    {
        private readonly MSSQLContext _context = context;

        public User ValidateCredentials(string username, string password)
        {
            var hashedPassword = HashPassword(password); // using centralized hash method
            return _context.Users.FirstOrDefault(u =>
                u.UserName == username &&
                u.Password == hashedPassword);
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

        public User Create(User user)
        {
            // Use centralized hash method
            user.Password = HashPassword(user.Password);

            // Initialize refresh token related fields
            user.RefreshToken = string.Empty;
            user.RefreshTokenExpiryTime = null;

            // Persist the new user
            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        // Centralized method for hashing passwords
        public string HashPassword(string password)
        {
            using var algorithm = SHA256.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

            var builder = new StringBuilder();
            foreach (var b in hashedBytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
