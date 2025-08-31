using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.IntegrationTests;
using RestWithASPNET10Erudio.Tests.IntegrationTests.Tools;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests
{
    [TestCaseOrderer("RestWithASPNET10Erudio.Tests.IntegrationTests.Tools.PriorityOrderer", "RestWithASPNET10Erudio.Tests")]
    public class PersonCorsIntegrationTests : IClassFixture<SqlServerFixture>
    {
        private readonly HttpClient _httpClient;
        private static PersonDTO _person;

        public PersonCorsIntegrationTests(SqlServerFixture fixture)
        {
            var factory = new CustomWebApplicationFactory<Program>(fixture.ConnectionString);
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost")
            });
        }

        private void AddOriginHeader(string origin)
        {
            _httpClient.DefaultRequestHeaders.Remove("Origin");
            _httpClient.DefaultRequestHeaders.Add("Origin", origin);
        }

        [Fact(DisplayName = "01 - Create Person With Allowed Origin")]
        [TestPriority(1)]
        public async Task CreatePerson_AllowedOrigin_ShouldReturnCreatedPerson()
        {
            AddOriginHeader("https://erudio.com.br");

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

            _person = created;
        }

        [Fact(DisplayName = "02 - Create Person With Disallowed Origin")]
        [TestPriority(2)]
        public async Task CreatePerson_DisallowedOrigin_ShouldReturnForbidden()
        {
            AddOriginHeader("https://semeru.com.br");

            var request = new PersonDTO
            {
                FirstName = "Test",
                LastName = "User",
                Address = "Some Address",
                Gender = "Other"
            };

            var response = await _httpClient.PostAsJsonAsync("api/person/v1", request);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("CORS origin not allowed.");
        }

        [Fact(DisplayName = "03 - Get Person By Id With Allowed Origin")]
        [TestPriority(3)]
        public async Task GetPersonById_AllowedOrigin_ShouldReturnPerson()
        {
            _person.Should().NotBeNull();

            AddOriginHeader("http://localhost:8080");
            var response = await _httpClient.GetAsync($"api/person/v1/{_person.Id}");
            response.EnsureSuccessStatusCode();

            var person = await response.Content.ReadFromJsonAsync<PersonDTO>();
            person.Should().NotBeNull();
            person!.Id.Should().Be(_person.Id);
            person.FirstName.Should().Be("Richard");
        }

        [Fact(DisplayName = "04 - Get Person By Id With Disallowed Origin")]
        [TestPriority(4)]
        public async Task GetPersonById_DisallowedOrigin_ShouldReturnForbidden()
        {
            _person.Should().NotBeNull();

            AddOriginHeader("https://semeru.com.br");
            var response = await _httpClient.GetAsync($"api/person/v1/{_person.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("CORS origin not allowed.");
        }
    }
}
