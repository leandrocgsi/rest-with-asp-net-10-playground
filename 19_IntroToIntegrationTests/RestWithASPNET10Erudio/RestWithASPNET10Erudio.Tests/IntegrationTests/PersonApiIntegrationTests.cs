using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Tests.IntegrationTests.Tools;
using System.Net.Http.Json;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests
{

    public class PersonApiIntegrationTests : IClassFixture<SqlServerFixture>
    {
        private readonly HttpClient _httpClient;

        public PersonApiIntegrationTests(SqlServerFixture fixture)
        {
            var factory = new CustomWebApplicationFactory(fixture.ConnectionString);
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost") // não precisa de 5000, o TestServer resolve
            });
        }

        [Fact]
        public async Task CreatePerson_ShouldReturnCreatedPerson()
        {
            var request = new
            {
                FirstName = "John",
                LastName = "Doe",
                Address = "NY",
                Gender = "Male"
            };

            var response = await _httpClient.PostAsJsonAsync("api/person", request);
            response.EnsureSuccessStatusCode();

            var person = await response.Content.ReadFromJsonAsync<PersonDTO>();
            person.Should().NotBeNull();
            person!.FirstName.Should().Be("John");
        }
    }
}
