using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestWithASPNET10Erudio.Model;
using RestWithASPNET10Erudio.Services;

namespace RestWithASPNET10Erudio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private IPersonServices _personService;
        private readonly ILogger<PersonController> _logger;

        public PersonController(IPersonServices personService, ILogger<PersonController> logger)
        {
            _personService = personService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Fetching all persons");
            return Ok(_personService.FindAll());
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            _logger.LogInformation("Fetching person with ID {Id}", id);
            var person = _personService.FindById(id);
            if (person == null)
            {
                _logger.LogWarning("Person with ID {Id} not found", id);
                return NotFound();
            }
            return Ok(person);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Person person)
        {
            _logger.LogInformation("Creating new person: {FirstName} {LastName}", person.FirstName, person.LastName);
            var createdPerson = _personService.Create(person);
            if (createdPerson == null)
            {
                _logger.LogError("Error creating person");
                return NotFound();
            }
            return Ok(createdPerson);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Person person)
        {
            _logger.LogInformation("Updating person ID {Id}", person.Id);
            var updatedPerson = _personService.Update(person);
            if (updatedPerson == null)
            {
                _logger.LogWarning("Person ID {Id} not found for update", person.Id);
                return NotFound();
            }
            _logger.LogDebug("Person updated successfully: {@Person}", updatedPerson);
            return Ok(updatedPerson);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Deleting person ID {Id}", id);
            _personService.Delete(id);
            _logger.LogDebug("Person ID {Id} deleted successfully", id);
            return NoContent();
        }
    }
}
