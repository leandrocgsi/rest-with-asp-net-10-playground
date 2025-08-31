using RestWithASPNET10Erudio.Model;

namespace RestWithASPNET10Erudio.Repositories
{
    public interface IPersonRepository : IRepository<Person>
    {
        public Person Disable(long id);
    }
}
