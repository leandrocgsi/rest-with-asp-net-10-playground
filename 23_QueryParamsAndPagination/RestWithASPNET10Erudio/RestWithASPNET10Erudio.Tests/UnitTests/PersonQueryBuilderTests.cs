using FluentAssertions;
using RestWithASPNET10Erudio.Repositories.QueryBuilders;

namespace RestWithASPNET10Erudio.Tests.UnitTests
{
    public class PersonQueryBuilderTests
    {
        private readonly PersonQueryBuilder _builder;

        public PersonQueryBuilderTests()
        {
            _builder = new PersonQueryBuilder();
        }

        private static string Normalize(string input)
        {
            if (input == null) return string.Empty;

            // Remove quebras de linha e tabs
            var withoutLines = input.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");

            // Substitui múltiplos espaços por apenas um
            while (withoutLines.Contains("  "))
                withoutLines = withoutLines.Replace("  ", " ");

            return withoutLines.Trim();
        }

        [Fact]
        public void BuildQueries_ShouldGenerateCorrectQueries_WithNameAndAscSort()
        {
            var (query, countQuery, sort, size, offset) =
                _builder.BuildQueries("John", "asc", 10, 2);

            sort.Should().Be("asc");
            size.Should().Be(10);
            offset.Should().Be(10);

            Normalize(query).Should().Contain(
                "SELECT * FROM person p WHERE 1 = 1 AND (p.first_name LIKE '%John%')");

            Normalize(query).Should().Contain("ORDER BY p.first_name asc");
            Normalize(query).Should().Contain("OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY");

            Normalize(countQuery).Should().Contain(
                "SELECT COUNT(*) FROM person p WHERE 1 = 1 AND (p.first_name LIKE '%John%')");
        }

        [Fact]
        public void BuildQueries_ShouldDefaultToAsc_WhenSortIsInvalidOrNull()
        {
            var (_, _, sort1, _, _) = _builder.BuildQueries("Alice", "invalid", 5, 1);
            var (_, _, sort2, _, _) = _builder.BuildQueries("Alice", null, 5, 1);

            sort1.Should().Be("asc");
            sort2.Should().Be("asc");
        }

        [Fact]
        public void BuildQueries_ShouldDefaultPageSizeToOne_WhenPageSizeIsLessThanOne()
        {
            var (_, _, _, size, _) = _builder.BuildQueries("Maria", "desc", 0, 1);
            size.Should().Be(1);
        }

        [Fact]
        public void BuildQueries_ShouldHandleEmptyNameCorrectly()
        {
            var (query, countQuery, _, _, _) = _builder.BuildQueries("", "desc", 10, 1);

            Normalize(query).Should().Contain("FROM person p WHERE 1 = 1");
            Normalize(query).Should().Contain("ORDER BY p.first_name desc");
            Normalize(query).Should().NotContain("first_name LIKE");

            Normalize(countQuery).Should().Contain("SELECT COUNT(*) FROM person p WHERE 1 = 1");
        }

        [Fact]
        public void BuildQueries_ShouldForcePageMinimumToOne()
        {
            var (_, _, _, _, offset) = _builder.BuildQueries("Leo", "asc", 5, 0);
            offset.Should().Be(0);
        }

        [Fact]
        public void BuildQueries_ShouldCalculateCorrectOffset_ForHigherPages()
        {
            var (_, _, _, _, offset) = _builder.BuildQueries("Mark", "asc", 10, 3);
            offset.Should().Be(20);
        }
    }
}
