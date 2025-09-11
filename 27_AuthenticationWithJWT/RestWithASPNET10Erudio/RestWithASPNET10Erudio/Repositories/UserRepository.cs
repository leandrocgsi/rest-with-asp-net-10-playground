using RestWithASPNET10Erudio.Model;
using RestWithASPNET10Erudio.Model.Context;
using RestWithASPNET10Erudio.Repositories.Impl;

namespace RestWithASPNET10Erudio.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(MSSQLContext context) : base(context) { }

        public User GetByUsername(string username)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == username);
        }
    }
}
