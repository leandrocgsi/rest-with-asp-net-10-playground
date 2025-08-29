using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.IntegrationTests;
using RestWithASPNET10Erudio.Tests.IntegrationTests.Tools;
using System.Net.Http.Json;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests
{
    [TestCaseOrderer("RestWithASPNET10Erudio.IntegrationTests.PriorityOrderer", "RestWithASPNET10Erudio.Tests")]
    public class PersonApiIntegrationTests : IClassFixture<SqlServerFixture>
    {
        private readonly HttpClient _httpClient;
        private static PersonDTO _person;

        public PersonApiIntegrationTests(SqlServerFixture fixture)
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
                FirstName = "Richard",
                LastName = "Stallman",
                Address = "New York City - New York - USA",
                Gender = "Male"
            };

            var response = await _httpClient.PostAsJsonAsync("api/person/v1", request);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<PersonDTO>();
            created.Should().NotBeNull();
            created!.Id.Should().BeGreaterThan(0);

            _person = created; // salva para próximos testes
        }

        [Fact(DisplayName = "02 - Get Person By Id")]
        [TestPriority(2)]
        public async Task GetPersonById_ShouldReturnPerson()
        {
            _person.Should().NotBeNull();

            var response = await _httpClient.GetAsync($"api/person/v1/{_person.Id}");
            response.EnsureSuccessStatusCode();

            var person = await response.Content.ReadFromJsonAsync<PersonDTO>();
            person.Should().NotBeNull();
            person!.Id.Should().Be(_person.Id);
            person.FirstName.Should().Be("Richard");
        }

        [Fact(DisplayName = "03 - Update Person")]
        [TestPriority(3)]
        public async Task UpdatePerson_ShouldReturnUpdatedPerson()
        {
            _person.Should().NotBeNull();
            _person.FirstName = "Richard Updated";

            var response = await _httpClient.PutAsJsonAsync("api/person/v1", _person);
            response.EnsureSuccessStatusCode();

            var updated = await response.Content.ReadFromJsonAsync<PersonDTO>();
            updated.Should().NotBeNull();
            updated!.FirstName.Should().Be("Richard Updated");

            _person = updated;
        }

        [Fact(DisplayName = "04 - Delete Person")]
        [TestPriority(4)]
        public async Task DeletePerson_ShouldReturnNoContent()
        {
            _person.Should().NotBeNull();

            var response = await _httpClient.DeleteAsync($"api/person/v1/{_person.Id}");
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact(DisplayName = "05 - Get All Persons")]
        [TestPriority(5)]
        public async Task GetAllPersons_ShouldReturnList()
        {
            var response = await _httpClient.GetAsync("api/person/v1");
            response.EnsureSuccessStatusCode();

            var list = await response.Content.ReadFromJsonAsync<List<PersonDTO>>();
            list.Should().NotBeNull();
        }
    }
}