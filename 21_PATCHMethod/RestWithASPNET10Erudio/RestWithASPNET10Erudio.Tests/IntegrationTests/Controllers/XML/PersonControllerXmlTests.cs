using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.IntegrationTests;
using RestWithASPNET10Erudio.Tests.IntegrationTests.Tools;
using System.Net.Http.Headers;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests.Controllers.Person
{
    [TestCaseOrderer("RestWithASPNET10Erudio.Tests.IntegrationTests.Tools.PriorityOrderer", "RestWithASPNET10Erudio.Tests")]
    public class PersonControllerXmlTests : IClassFixture<SqlServerFixture>
    {
        private readonly HttpClient _httpClient;
        private static PersonDTO _person;

        public PersonControllerXmlTests(SqlServerFixture fixture)
        {
            var factory = new CustomWebApplicationFactory<Program>(fixture.ConnectionString);
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost")
            });

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        }

        [Fact(DisplayName = "01 - Create Person With XML")]
        [TestPriority(1)]
        public async Task CreatePerson_WithXml_ShouldReturnCreatedPerson()
        {
            var request = new PersonDTO
            {
                FirstName = "Linus",
                LastName = "Torvalds",
                Address = "Helsinki - Finland",
                Gender = "Male",
                Enabled = true
            };

            var response = await _httpClient.PostAsync(
                "api/person/v1",
                XmlHelper.SerializeToXml(request)
            );
            response.EnsureSuccessStatusCode();

            var created = await XmlHelper.DeserializeFromXmlAsync<PersonDTO>(response);
            created.Should().NotBeNull();
            created!.Id.Should().BeGreaterThan(0);
            created.FirstName.Should().Be("Linus");
            created.LastName.Should().Be("Torvalds");
            created.Address.Should().Be("Helsinki - Finland");
            created.Gender.Should().Be("Male");
            created.Enabled.Should().BeTrue();

            _person = created;
        }

        [Fact(DisplayName = "02 - Update Person With XML")]
        [TestPriority(2)]
        public async Task UpdatePerson_WithXml_ShouldReturnUpdatedPerson()
        {
            _person.LastName = "Benedict Torvalds";

            var response = await _httpClient.PutAsync(
                "api/person/v1",
                XmlHelper.SerializeToXml(_person)
            );
            response.EnsureSuccessStatusCode();

            var updated = await XmlHelper.DeserializeFromXmlAsync<PersonDTO>(response);
            updated.Should().NotBeNull();
            updated!.Id.Should().Be(_person.Id);
            updated.FirstName.Should().Be("Linus");
            updated.LastName.Should().Be("Benedict Torvalds");
            updated.Address.Should().Be("Helsinki - Finland");
            updated.Gender.Should().Be("Male");
            updated.Enabled.Should().BeTrue();

            _person = updated;
        }

        [Fact(DisplayName = "03 - Disable Person With XML")]
        [TestPriority(3)]
        public async Task DisablePerson_WithXml_ShouldReturnDisabledPerson()
        {
            var response = await _httpClient.PatchAsync(
                $"api/person/v1/{_person.Id}",
                null
            );
            response.EnsureSuccessStatusCode();

            var patched = await XmlHelper.DeserializeFromXmlAsync<PersonDTO>(response);
            patched.Should().NotBeNull();
            patched!.Id.Should().Be(_person.Id);
            patched.Enabled.Should().BeFalse();

            _person = patched;
        }

        [Fact(DisplayName = "04 - Get Person By Id With XML")]
        [TestPriority(4)]
        public async Task GetPersonById_WithXml_ShouldReturnPerson()
        {
            var response = await _httpClient.GetAsync($"api/person/v1/{_person.Id}");
            response.EnsureSuccessStatusCode();

            var person = await XmlHelper.DeserializeFromXmlAsync<PersonDTO>(response);
            person.Should().NotBeNull();
            person!.Id.Should().Be(_person.Id);
            person.FirstName.Should().Be("Linus");
            person.LastName.Should().Be("Benedict Torvalds");
            person.Address.Should().Be("Helsinki - Finland");
            person.Gender.Should().Be("Male");
            person.Enabled.Should().BeFalse();
        }

        [Fact(DisplayName = "05 - Delete Person With XML")]
        [TestPriority(5)]
        public async Task DeletePerson_WithXml_ShouldReturnNoContent()
        {
            var response = await _httpClient.DeleteAsync($"api/person/v1/{_person.Id}");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact(DisplayName = "06 - Get All Persons With XML")]
        [TestPriority(6)]
        public async Task GetAllPersons_WithXml_ShouldReturnList()
        {
            var response = await _httpClient.GetAsync("api/person/v1");
            response.EnsureSuccessStatusCode();

            var list = await XmlHelper.DeserializeFromXmlAsync<List<PersonDTO>>(response);
            list.Should().NotBeNull();
            list.Should().NotBeEmpty();

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