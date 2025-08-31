using RestWithASPNET10Erudio.Model;
using RestWithASPNET10Erudio.Model.Context;
using RestWithASPNET10Erudio.Repositories.Impl;

namespace RestWithASPNET10Erudio.Repositories
{
    //public class PersonRepository : GenericRepository<Person>, IPersonRepository
    public class PersonRepository(MSSQLContext context)
        : GenericRepository<Person>(context), IPersonRepository
    {

        //public PersonRepository(MSSQLContext context) : base(context) { }
        public Person Disable(long id)
        {
            var user = _context.Persons.Find(id);
            if (user == null) return null;

            user.Enabled = false;
            _context.SaveChanges();

            return user;
        }
    }
}