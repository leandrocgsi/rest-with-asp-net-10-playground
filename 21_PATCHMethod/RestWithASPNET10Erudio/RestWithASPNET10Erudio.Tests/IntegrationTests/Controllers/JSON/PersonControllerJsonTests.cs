using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.IntegrationTests;
using RestWithASPNET10Erudio.Tests.IntegrationTests.Tools;
using System.Net.Http.Json;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests.Controllers.JSON
{
    [TestCaseOrderer("RestWithASPNET10Erudio.Tests.IntegrationTests.Tools.PriorityOrderer", "RestWithASPNET10Erudio.Tests")]
    public class PersonControllerJsonTests : IClassFixture<SqlServerFixture>
    {
        private readonly HttpClient _httpClient;
        private static PersonDTO _person;

        public PersonControllerJsonTests(SqlServerFixture fixture)
        {
            var factory = new CustomWebApplicationFactory<Program>(fixture.ConnectionString);
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost")
            });
        }

        [Fact(DisplayName = "01 - Create Person")]
        [TestPriority(1)]
        public async Task CreatePerson_ShouldReturnCreatedPerson()
        {
            var request = new PersonDTO
            {
                FirstName = "Linus",
                LastName = "Torvalds",
                Address = "Helsinki - Finland",
                Gender = "Male",
                Enabled = true
            };

            var response = await _httpClient.PostAsJsonAsync("api/person/v1", request);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<PersonDTO>();
            created.Should().NotBeNull();
            created!.Id.Should().BeGreaterThan(0);
            created.FirstName.Should().Be("Linus");
            created.LastName.Should().Be("Torvalds");
            created.Address.Should().Be("Helsinki - Finland");
            created.Gender.Should().Be("Male");
            created.Enabled.Should().BeTrue();

            _person = created;
        }

        [Fact(DisplayName = "02 - Update Person")]
        [TestPriority(2)]
        public async Task UpdatePerson_ShouldReturnUpdatedPerson()
        {
            _person.LastName = "Benedict Torvalds";

            var response = await _httpClient.PutAsJsonAsync("api/person/v1", _person);
            response.EnsureSuccessStatusCode();

            var updated = await response.Content.ReadFromJsonAsync<PersonDTO>();
            updated.Should().NotBeNull();
            updated!.Id.Should().Be(_person.Id);
            updated.FirstName.Should().Be("Linus");
            updated.LastName.Should().Be("Benedict Torvalds");
            updated.Address.Should().Be("Helsinki - Finland");
            updated.Gender.Should().Be("Male");
            updated.Enabled.Should().BeTrue();

            _person = updated;
        }

        [Fact(DisplayName = "03 - Disable Person")]
        [TestPriority(3)]
        public async Task DisablePerson_ShouldReturnDisabledPerson()
        {
            var response = await _httpClient.PatchAsync($"api/person/v1/{_person.Id}", null);
            response.EnsureSuccessStatusCode();

            var patched = await response.Content.ReadFromJsonAsync<PersonDTO>();
            patched.Should().NotBeNull();
            patched!.Id.Should().Be(_person.Id);
            patched.FirstName.Should().Be("Linus");
            patched.LastName.Should().Be("Benedict Torvalds");
            patched.Address.Should().Be("Helsinki - Finland");
            patched.Gender.Should().Be("Male");
            patched.Enabled.Should().BeFalse();

            _person = patched;
        }

        [Fact(DisplayName = "04 - Get Person By Id")]
        [TestPriority(4)]
        public async Task GetPersonById_ShouldReturnPerson()
        {
            var response = await _httpClient.GetAsync($"api/person/v1/{_person.Id}");
            response.EnsureSuccessStatusCode();

            var person = await response.Content.ReadFromJsonAsync<PersonDTO>();
            person.Should().NotBeNull();
            person!.Id.Should().Be(_person.Id);
            person.FirstName.Should().Be("Linus");
            person.LastName.Should().Be("Benedict Torvalds");
            person.Address.Should().Be("Helsinki - Finland");
            person.Gender.Should().Be("Male");
            person.Enabled.Should().BeFalse(); // já desabilitado
        }

        [Fact(DisplayName = "05 - Delete Person")]
        [TestPriority(5)]
        public async Task DeletePerson_ShouldReturnNoContent()
        {
            var response = await _httpClient.DeleteAsync($"api/person/v1/{_person.Id}");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact(DisplayName = "06 - Get All Persons")]
        [TestPriority(6)]
        public async Task GetAllPersons_ShouldReturnList()
        {
            var response = await _httpClient.GetAsync("api/person/v1");
            response.EnsureSuccessStatusCode();

            var list = await response.Content.ReadFromJsonAsync<List<PersonDTO>>();
            list.Should().NotBeNull();
            list!.Should().NotBeEmpty();

            // Exemplo de validação mais detalhada como no Java
            var first = list.First(p => p.FirstName == "Ayrton");
            first.LastName.Should().Be("Senna");
            first.Address.Should().Be("São Paulo - Brasil");
            first.Gender.Should().Be("Male");
            first.Enabled.Should().BeTrue();

            /*
            var second = list.First(p => p.FirstName == "Nikola");
            second.LastName.Should().Be("Tesla");
            second.Address.Should().Be("Smiljan - Croatia");
            second.Gender.Should().Be("Male");
            second.Enabled.Should().BeTrue();
            */
        }
    }
}