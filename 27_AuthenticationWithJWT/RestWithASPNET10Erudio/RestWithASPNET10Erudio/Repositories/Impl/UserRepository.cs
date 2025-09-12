using RestWithASPNET10Erudio.Model;
using RestWithASPNET10Erudio.Model.Context;

namespace RestWithASPNET10Erudio.Repositories.Impl
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(MSSQLContext context) : base(context) { }

        public User FindByUsername(string username)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == username);
        }
    }
}
