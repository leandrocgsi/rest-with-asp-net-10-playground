using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RestWithASPNET10Erudio.IntegrationTests;
using RestWithASPNET10Erudio.Tests.IntegrationTests.Tools;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests
{
    public class ScalarDocumentationIntegrationTests : IClassFixture<SqlServerFixture>
    {
        private readonly HttpClient _httpClient;

        public ScalarDocumentationIntegrationTests(SqlServerFixture sqlFixture)
        {
            var factory = new CustomWebApplicationFactory<Program>(sqlFixture.ConnectionString);

            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost")
            });
        }

        [Fact(DisplayName = "Scalar UI should be accessible and return HTML")]
        public async Task ScalarUi_ShouldBeAccessible()
        {
            var response = await _httpClient.GetAsync("/scalar/");
            response.EnsureSuccessStatusCode(); // garante 200 OK

            var content = await response.Content.ReadAsStringAsync();

            // Verifica que contém o título esperado da documentação Scalar
            content.Should().Contain("<title>ASP.NET 2026 REST API's from 0 to Azure and GCP with .NET 10, Docker e Kubernetes</title>");

            // Opcional: verifica que o HTML contém algum script característico da UI
            content.Should().Contain("script src");
        }
    }
}
