using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.File.Exporters.Factory;
using RestWithASPNET10Erudio.File.Importers.Factory;
using RestWithASPNET10Erudio.Hypermedia.Utils;
using RestWithASPNET10Erudio.Model;
using RestWithASPNET10Erudio.Repositories;

namespace RestWithASPNET10Erudio.Services.Impl
{
    public class PersonServicesImpl : IPersonServices
    {
        private readonly IPersonRepository _repository;
        private readonly ILogger<PersonServicesImpl> _logger;

        private readonly FileImporterFactory _importerFactory;
        private readonly FileExporterFactory _exporterFactory;

        public PersonServicesImpl(
            FileImporterFactory importerFactory,
            FileExporterFactory exporterFactory,
            IPersonRepository repository,
            ILogger<PersonServicesImpl> logger)
        {
            _importerFactory = importerFactory;
            _exporterFactory = exporterFactory;
            _repository = repository;
            _logger = logger;
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
            return _repository.FindByName(firstName, lastName)
                .Adapt<List<PersonDTO>>();
        }

        public PagedSearchDTO<PersonDTO> FindWithPagedSearch(
            string name,
            string sortDirection,
            int pageSize,
            int page)
        {
            var result = _repository.FindWithPagedSearch(name, sortDirection, pageSize, page);
            return result.Adapt<PagedSearchDTO<PersonDTO>>();
        }

        public async Task<List<PersonDTO>> MassCreationAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Empty file received.");
                return [];
            }

            using var stream = file.OpenReadStream();
            var fileName = file.FileName;

            try
            {
                var importer = _importerFactory.GetImporter(fileName);
                var people = await importer.ImportFileAsync(stream);

                // persiste no banco e retorna DTOs atualizados
                var entities = people
                    .Select(dto => _repository.Create(
                        dto.Adapt<Person>()))
                    .ToList();

                return entities.Adapt<List<PersonDTO>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while importing file {FileName}", fileName);
                throw;
            }
        }

        public FileContentResult ExportPage(
            int page,
            int pageSize,
            string sortDirection,
            string acceptHeader,
            string name)
        {
            _logger.LogInformation("Exporting People page {Page}, size {Size}, direction {Direction}", page, pageSize, sortDirection);

            var content = _repository.FindWithPagedSearch(name, sortDirection, pageSize, page);
            try
            {
                var exporter = _exporterFactory.GetExporter(acceptHeader);
                var people = content.List.Adapt<List<PersonDTO>>();
                return exporter.ExportFile(people);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting People data");
                throw;
            }
        }
    }
}
