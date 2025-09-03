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

        public PagedSearchDTO<PersonDTO> FindWithPagedSearch(
            string name,
            string sortDirection,
            int pageSize,
            int page)
        {
            // Chama BuildQueries para gerar as queries completas
            var (query, countQuery, sort, size, offset) = BuildQueries(name, sortDirection, pageSize, page);

            // Executa as queries no repositório
            var persons = _repository.FindWithPagedSearch(query);
            var totalResults = _repository.GetCount(countQuery);

            // Retorna DTO com paginação
            return new PagedSearchDTO<PersonDTO>
            {
                CurrentPage = page,
                List = persons.Adapt<List<PersonDTO>>(),
                PageSize = size,
                SortDirections = sort,
                TotalResults = totalResults
            };
        }

        // Responsável apenas por montar queries
        public (string Query, string CountQuery, string Sort, int Size, int Offset) BuildQueries(
            string name, string sortDirection, int pageSize, int page)
        {
            // ===== Mudança: página mínima = 1 =====
            page = Math.Max(1, page);

            // Offset para SQL Server
            var offset = (page - 1) * pageSize;

            // PageSize mínimo = 1
            var size = pageSize < 1 ? 1 : pageSize;

            // ===== Mudança: regra de sort atualizada =====
            var sort = string.Equals(
                sortDirection, "desc",
                StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";

            // Montagem da query base
            var baseQuery = "FROM person p WHERE 1 = 1";
            if (!string.IsNullOrWhiteSpace(name))
                baseQuery += $" AND p.first_name LIKE '%{name}%'";

            // Query de dados completa para SQL Server
            var query = $@"
                SELECT * {baseQuery}
                ORDER BY p.first_name {sort}
                OFFSET {offset} ROWS FETCH NEXT {size} ROWS ONLY";

            // Query de contagem
            var countQuery = $"SELECT COUNT(*) {baseQuery}";

            return (query, countQuery, sort, size, offset);
        }

    }
}
