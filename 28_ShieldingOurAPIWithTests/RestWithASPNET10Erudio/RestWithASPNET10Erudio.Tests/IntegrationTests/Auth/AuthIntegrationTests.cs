using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Tests.IntegrationTests.Tools;
using RestWithASPNETErudio.Data.DTO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests.Auth
{
    [TestCaseOrderer(
        TestConfigs.TestCaseOrdererFullName,
        TestConfigs.TestCaseOrdererAssembly)]
    public class AuthIntegrationTests : IClassFixture<SqlServerFixture>
    {
        private readonly HttpClient _httpClient;
        private static TokenDTO _token;
        private static AccountCredentialsDTO _createdUser;

        public AuthIntegrationTests(SqlServerFixture sqlFixture)
        {
            var factory = new CustomWebApplicationFactory<Program>(
                sqlFixture.ConnectionString);

            _httpClient = factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new Uri("http://localhost")
                }
            );
        }

        [Fact(DisplayName = "01 - Create User")]
        [TestPriority(1)]
        public async Task CreateUser_ShouldReturnOk()
        {
            // Arrange
            var request = new AccountCredentialsDTO
            {
                Username = "solomon",
                FullName = "Solomon Hykes",
                Password = "hykes123"
            };

            // Act
            var response = await _httpClient
                .PostAsJsonAsync("api/auth/create", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<AccountCredentialsDTO>();

            result.Should().NotBeNull();
            result.Username.Should().Be("solomon");
            result.FullName.Should().Be("Solomon Hykes");

            _createdUser = request;
        }

        [Fact(DisplayName = "02 - SignIn")]
        [TestPriority(2)]
        public async Task SignIn_ShouldReturnToken()
        {
            // Arrange
            var credentials = new UserDTO
            {
                Username = _createdUser.Username,
                Password = _createdUser.Password
            };

            // Act
            var response = await _httpClient
                .PostAsJsonAsync("api/auth/signin", credentials);

            // Assert
            response.EnsureSuccessStatusCode();
            var token = await response.Content.ReadFromJsonAsync<TokenDTO>();

            token.Should().NotBeNull();
            token.AccessToken.Should().NotBeNullOrEmpty();
            token.RefreshToken.Should().NotBeNullOrEmpty();

            _token = token;
        }

        [Fact(DisplayName = "03 - Refresh Token")]
        [TestPriority(3)]
        public async Task Refresh_ShouldReturnNewToken()
        {
            // Act
            var response = await _httpClient
                .PostAsJsonAsync("api/auth/refresh", _token);

            // Assert
            response.EnsureSuccessStatusCode();
            var refreshed = await response.Content.ReadFromJsonAsync<TokenDTO>();

            refreshed.Should().NotBeNull();
            refreshed.AccessToken.Should().NotBeNullOrEmpty();
            refreshed.RefreshToken.Should().NotBeNullOrEmpty();

            _token = refreshed;
        }

        [Fact(DisplayName = "04 - Revoke Token")]
        [TestPriority(4)]
        public async Task Revoke_ShouldReturnNoContent()
        {
            // Arrange
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token.AccessToken);

            // Act
            var response = await _httpClient.PostAsync("api/auth/revoke", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact(DisplayName = "05 - SignIn with Invalid Credentials ShouldReturnUnauthorized")]
        [TestPriority(5)]
        public async Task SignIn_InvalidCredentials_ShouldReturnUnauthorized()
        {
            var invalidCredentials = new UserDTO
            {
                Username = "wronguser",
                Password = "wrongpass"
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/signin", invalidCredentials);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "06 - Revoke without Authorization Header ShouldReturnUnauthorized")]
        [TestPriority(6)]
        public async Task Revoke_WithoutToken_ShouldReturnUnauthorized()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            var response = await _httpClient.PostAsync("api/auth/revoke", null);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }


    }
}
