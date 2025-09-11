using RestWithASPNET10Erudio.Model;

namespace RestWithASPNET10Erudio.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User? GetByUsername(string username);
    }
}
