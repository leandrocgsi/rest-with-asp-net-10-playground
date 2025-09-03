using FluentAssertions;
using RestWithASPNET10Erudio.Services.Impl;

namespace RestWithASPNET10Erudio.Tests.UnitTests
{
    public class PersonServicesImplQueryTests
    {
        private readonly PersonServicesImpl _service;

        public PersonServicesImplQueryTests()
        {
            // Passa null para o repositório, não será usado
            _service = new PersonServicesImpl(null);
        }

        [Fact]
        public void BuildQueries_ShouldGenerateCorrectQueries_WithNameAndAscSort()
        {
            // Arrange
            string name = "John";
            string sortDirection = "asc";
            int pageSize = 10;
            int page = 2;

            // Act
            var (query, countQuery, sort, size, offset) =
                _service.BuildQueries(name, sortDirection, pageSize, page);

            // Assert
            sort.Should().Be("asc");
            size.Should().Be(10);
            offset.Should().Be(10);

            query.Should().ContainEquivalentOf("SELECT * FROM person p WHERE 1 = 1 AND p.first_name LIKE '%John%'");
            query.Should().ContainEquivalentOf("ORDER BY p.first_name asc");
            query.Should().ContainEquivalentOf("OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY");

            countQuery.Should().ContainEquivalentOf("SELECT COUNT(*) FROM person p WHERE 1 = 1 AND p.first_name LIKE '%John%'");
        }

        [Fact]
        public void BuildQueries_ShouldDefaultToAsc_WhenSortIsInvalidOrNull()
        {
            var (_, _, sort1, _, _) = _service.BuildQueries("Alice", "invalid", 5, 1);
            var (_, _, sort2, _, _) = _service.BuildQueries("Alice", null, 5, 1);

            sort1.Should().Be("asc");
            sort2.Should().Be("asc");
        }

        [Fact]
        public void BuildQueries_ShouldDefaultPageSizeToOne_WhenPageSizeIsLessThanOne()
        {
            var (_, _, _, size, _) = _service.BuildQueries("Maria", "desc", 0, 1);
            size.Should().Be(1);
        }

        [Fact]
        public void BuildQueries_ShouldHandleEmptyNameCorrectly()
        {
            var (query, countQuery, _, _, _) = _service.BuildQueries("", "desc", 10, 1);

            query.Should().ContainEquivalentOf("FROM person p WHERE 1 = 1");
            query.Should().ContainEquivalentOf("ORDER BY p.first_name desc");
            query.Should().NotContainEquivalentOf("first_name LIKE");

            countQuery.Should().ContainEquivalentOf("SELECT COUNT(*) FROM person p WHERE 1 = 1");
        }
    }
}
