using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Tests.IntegrationTests.Tools;
using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace RestWithASPNET10Erudio.Tests.IntegrationTests.HATEOAS
{
    [TestCaseOrderer(
        TestConstants.TestCaseOrdererFullName,
        TestConstants.TestCaseOrdererAssembly)]
    public class PersonControllerHateoasRegexTests : IClassFixture<SqlServerFixture>
    {
        private readonly HttpClient _httpClient;
        private static PersonDTO _person;

        public PersonControllerHateoasRegexTests(SqlServerFixture sqlFixture)
        {
            var factory = new CustomWebApplicationFactory<Program>(sqlFixture.ConnectionString);
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost")
            });
        }

        private void AssertLinkPattern(string content, string rel)
        {
            var pattern = $@"""rel"":\s*""{rel}"".*?""href"":\s*""https?://.+/api/person/v1.*?""";
            Regex.IsMatch(content, pattern).Should().BeTrue($"Link with rel='{rel}' should exist and have valid href");
        }

        [Fact(DisplayName = "01 - Create Person")]
        [TestPriority(1)]
        public async Task CreatePerson_ShouldContainHateoasLinks()
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

            var content = await response.Content.ReadAsStringAsync();
            _person = await response.Content.ReadFromJsonAsync<PersonDTO>();

            AssertLinkPattern(content, "collection");
            AssertLinkPattern(content, "self");
            AssertLinkPattern(content, "create");
            AssertLinkPattern(content, "update");
            AssertLinkPattern(content, "delete");
        }

        [Fact(DisplayName = "02 - Update Person")]
        [TestPriority(2)]
        public async Task UpdatePerson_ShouldContainHateoasLinks()
        {
            _person.LastName = "Benedict Torvalds";

            var response = await _httpClient.PutAsJsonAsync("api/person/v1", _person);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            _person = await response.Content.ReadFromJsonAsync<PersonDTO>();

            AssertLinkPattern(content, "collection");
            AssertLinkPattern(content, "self");
            AssertLinkPattern(content, "create");
            AssertLinkPattern(content, "update");
            AssertLinkPattern(content, "delete");
        }

        [Fact(DisplayName = "03 - Disable Person By ID")]
        [TestPriority(3)]
        public async Task DisablePersonById_ShouldContainHateoasLinks()
        {
            var response = await _httpClient.PatchAsync($"api/person/v1/{_person.Id}", null);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            _person = await response.Content.ReadFromJsonAsync<PersonDTO>();

            AssertLinkPattern(content, "collection");
            AssertLinkPattern(content, "self");
            AssertLinkPattern(content, "create");
            AssertLinkPattern(content, "update");
            AssertLinkPattern(content, "delete");
        }

        [Fact(DisplayName = "04 - Get Person By ID")]
        [TestPriority(4)]
        public async Task GetPersonById_ShouldContainHateoasLinks()
        {
            var response = await _httpClient.GetAsync($"api/person/v1/{_person.Id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            AssertLinkPattern(content, "collection");
            AssertLinkPattern(content, "self");
            AssertLinkPattern(content, "create");
            AssertLinkPattern(content, "update");
            AssertLinkPattern(content, "delete");
        }

        [Fact(DisplayName = "05 - Delete Person By ID")]
        [TestPriority(5)]
        public async Task DeletePersonById_ShouldReturnNoContent()
        {
            var response = await _httpClient.DeleteAsync($"api/person/v1/{_person.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact(DisplayName = "06 - Find All Persons")]
        [TestPriority(6)]
        public async Task FindAll_ShouldReturnLinksForEachPerson()
        {
            // Act
            var response = await _httpClient.GetAsync("api/person/v1");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            // Regex para capturar todos os "id" do retorno
            var idMatches = Regex.Matches(content, @"""id"":\s*(\d+)");
            idMatches.Count.Should().BeGreaterThan(0, "There should be at least one person");

            foreach (Match match in idMatches)
            {
                var id = match.Groups[1].Value;

                // Validar cada link do item
                var expectedRels = new[] { "collection", "self", "create", "update", "delete" };

                foreach (var rel in expectedRels)
                {
                    var pattern = rel switch
                    {
                        "self" or "delete" => $@"""rel"":\s*""{rel}"".*?""href"":\s*""https?://.+/api/person/v1/{id}""",
                        _ => $@"""rel"":\s*""{rel}"".*?""href"":\s*""https?://.+/api/person/v1"""
                    };

                    Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase)
                         .Should().BeTrue($"Link '{rel}' should exist for person {id}");

                    // Validar que existe type
                    var typePattern = $@"""rel"":\s*""{rel}"".*?""type"":\s*""[^""]+""";
                    Regex.IsMatch(content, typePattern).Should().BeTrue($"Link '{rel}' must have a type for person {id}");
                }
            }
        }
    }
}