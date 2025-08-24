using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using RestWithASPNET10Erudio.Data.DTO.V2;
using RestWithASPNET10Erudio.Services.Impl;

namespace RestWithASPNET10Erudio.Controllers.V2
{
    [ApiVersion("2")]
    [ApiController]
    [Route("api/person/v{version:apiVersion}")]
    public class PersonController : ControllerBase
    {
        private readonly PersonServicesImplV2 _service;
        private readonly ILogger<PersonController> _logger;

        public PersonController(PersonServicesImplV2 service,
            ILogger<PersonController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        [MapToApiVersion("2")]
        public IActionResult Create([FromBody] PersonDTO person)
        {
            _logger.LogInformation("Creating new Person: {firstName}", person.FirstName);

            var createdPerson = _service.Create(person);
            if (createdPerson == null)
            {
                _logger.LogError("Failed to create person with name {firstName}", person.FirstName);
                return NotFound();
            }
            return Ok(createdPerson);
        }
    }
}