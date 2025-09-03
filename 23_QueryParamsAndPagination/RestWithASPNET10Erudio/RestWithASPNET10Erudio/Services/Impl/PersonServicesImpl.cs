using Mapster;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Hypermedia.Utils;
using RestWithASPNET10Erudio.Model;
using RestWithASPNET10Erudio.Repositories;
using System;

namespace RestWithASPNET10Erudio.Services.Impl
{
    public class PersonServicesImpl : IPersonServices
    {

        private IPersonRepository _repository;
        public PersonServicesImpl(IPersonRepository repository)
        {
            _repository = repository;
        }

        public List<PersonDTO> FindAll()
        {
            return _repository.FindAll().Adapt<List<PersonDTO>>();
        }

        public PersonDTO FindById(long id)
        {
            return _repository.FindById(id).Adapt<PersonDTO>();
        }

        public PersonDTO Create(PersonDTO person)
        {
            var entity = person.Adapt<Person>();
            entity = _repository.Create(entity);
            return entity.Adapt<PersonDTO>();
        }

        public PersonDTO Update(PersonDTO person)
        {
            var entity = person.Adapt<Person>();
            entity = _repository.Update(entity);
            return entity.Adapt<PersonDTO>();
        }
        public void Delete(long id)
        {
            _repository.Delete(id);
        }

        public PersonDTO Disable(long id)
        {
            var entity = _repository.Disable(id);
            return entity.Adapt<PersonDTO>();
        }

        public List<PersonDTO> FindByName(string firstName, string lastName)
        {
            return _repository
                .FindByName(firstName, lastName)
                .Adapt<List<PersonDTO>>();
        }

        public PagedSearchDTO<PersonDTO> FindWithPagedSearch(string name, string sortDirection, int pageSize, int page)
        {
            var (query, countQuery, sort, size, offset)
                = BuildQueries(name, sortDirection, pageSize, page);

            var persons = _repository.FindWithPagedSearch(query);
            var totalResults = _repository.GetCount(countQuery);

            return new PagedSearchDTO<PersonDTO>
            {
                CurrentPage = page,
                List = persons.Adapt<List<PersonDTO>>(),
                PageSize = size,
                SortDirections = sort,
                TotalResults = totalResults
            };
        }

        // Novo método isolado só para montar queries
        public (string Query, string CountQuery, string Sort, int Size, int Offset)
            BuildQueries(string name, string sortDirection, int pageSize, int page)
        {
            var offset = page > 0 ? (page - 1) * pageSize : 0;
            var sort = !string.IsNullOrEmpty(sortDirection) &&
                       sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                       ? "desc" : "asc";
            var size = pageSize < 1 ? 1 : pageSize;

            var baseQuery = "from person p where 1 = 1";
            if (!string.IsNullOrEmpty(name))
                baseQuery += $" and p.first_name like '%{name}%'";

            var query = $@"
            select * {baseQuery}
            order by p.first_name {sort}
            offset {offset} rows fetch next {size} rows only";

            var countQuery = $"select count(*) {baseQuery}";

            return (query, countQuery, sort, size, offset);
        }
    }
}
