using FluentAssertions;
using RestWithASPNET10Erudio.IntegrationTests;
using RestWithASPNET10Erudio.Tests.IntegrationTests.Tools;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests
{
    public class ScalarDocumentationIntegrationTests : IClassFixture<SqlServerFixture>
    {
        private readonly HttpClient _httpClient;

        public ScalarDocumentationIntegrationTests(SqlServerFixture sqlFixture)
        {
            // Passa a connection string do Testcontainers para o factory
            var factory = new CustomWebApplicationFactory<Program>(sqlFixture.ConnectionString);

            // Cria o client diretamente da instância em memória do servidor de testes
            _httpClient = factory.CreateClient();
        }

        [Fact(DisplayName = "Scalar UI should be accessible and return HTML")]
        public async Task ScalarUi_ShouldBeAccessible()
        {
            // Act
            var response = await _httpClient.GetAsync("/scalar/");
            response.EnsureSuccessStatusCode(); // garante 200 OK

            var content = await response.Content.ReadAsStringAsync();

            // Assert
            content.Should().Contain("<title>ASP.NET 2026 REST API's from 0 to Azure and GCP with .NET 10, Docker e Kubernetes</title>");
            content.Should().Contain("script src");
        }
    }
}
