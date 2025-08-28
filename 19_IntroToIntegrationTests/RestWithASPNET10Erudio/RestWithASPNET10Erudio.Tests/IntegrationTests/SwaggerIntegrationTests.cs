using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RestWithASPNET10Erudio.IntegrationTests;
using RestWithASPNET10Erudio.Tests.IntegrationTests.Tools;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests
{
    public class SwaggerIntegrationTests :
    IClassFixture<SqlServerFixture>
    {
        private readonly HttpClient _httpClient;

        public SwaggerIntegrationTests(SqlServerFixture sqlFixture)
        {
            var factory = new CustomWebApplicationFactory<Program>(sqlFixture.ConnectionString);

            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost") // TestServer usa porta dinâmica
            });
        }

        [Fact]
        public async Task Swagger_ShouldBeAvailable()
        {
            var response = await _httpClient.GetAsync("/swagger/v1/swagger.json");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
            content.Should().Contain("api/person/v1");
        }

        [Fact]
        public async Task SwaggerUi_ShouldBeAccessible()
        {
            var response = await _httpClient.GetAsync("/swagger-ui/index.html");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("<div id=\"swagger-ui\"></div>");
        }
    }
}