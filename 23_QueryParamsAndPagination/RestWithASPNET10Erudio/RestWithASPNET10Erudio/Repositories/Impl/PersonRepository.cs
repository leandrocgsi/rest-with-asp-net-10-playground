using RestWithASPNET10Erudio.Model;
using RestWithASPNET10Erudio.Model.Context;
using RestWithASPNET10Erudio.Repositories.QueryBuilders;

namespace RestWithASPNET10Erudio.Repositories.Impl
{
    public class PersonRepository(MSSQLContext context)
        : GenericRepository<Person>(context), IPersonRepository
    {
        public Person Disable(long id)
        {
            var person = _context.Persons.Find(id);
            if (person == null) return null;
            person.Enabled = false;
            _context.SaveChanges();
            return person;
        }

        public List<Person> FindByName(
            string firstName, string lastName)
        {
            var query = _context.Persons.AsQueryable();
            if (!string.IsNullOrWhiteSpace(firstName))
                query = query.Where(p => p.FirstName.Contains(firstName));
            if (!string.IsNullOrWhiteSpace(lastName))
                query = query.Where(p => p.LastName.Contains(lastName));
            // return query.ToList();
            return [.. query];
        }

        public PagedSearch<Person> FindWithPagedSearch(string name, string sortDirection, int pageSize, int page)
        {
            page = Math.Max(1, page);
            var builder = new PersonQueryBuilder();
            var (query, countQuery, sort, size, offset) = builder.BuildQueries(
                name, sortDirection, pageSize, page);

            var persons = base.FindWithPagedSearch(query);
            var totalResults = base.GetCount(countQuery);

            return new PagedSearch<Person>
            {
                CurrentPage = page,
                List = persons,
                PageSize = size,
                SortDirections = sort,
                TotalResults = totalResults
            };
        }

        /*
        public (
            string query,
            string countQuery,
            string sort,
            int size,
            int offset)
            BuildQueries(
                string name,
                string sortDirection,
                int pageSize,
                int page)
        {
            page = Math.Max(1, page);

            var offset = (page - 1) * pageSize;
            var size = pageSize < 1 ? 1 : pageSize;

            var sort = !string.IsNullOrWhiteSpace(sortDirection) &&
                !sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                ? "asc" : "desc";

            var whereClause = $" FROM person p WHERE 1 = 1 ";
            if (!string.IsNullOrWhiteSpace(name))
                whereClause += $" AND (p.first_name LIKE '%{name}%') ";

            var query = $@"
                SELECT * {whereClause}
                ORDER BY p.first_name {sort}
                OFFSET {offset} ROWS FETCH NEXT {size} ROWS ONLY";

            var countQuery = $"SELECT COUNT(*) {whereClause}";
            return (query, countQuery, sort, size, offset);
        }*/
    }
}
