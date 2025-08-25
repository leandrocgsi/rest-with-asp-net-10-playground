using FluentAssertions;
using RestWithASPNET10Erudio.Data.Converter.Impl;
using RestWithASPNET10Erudio.Data.DTO.V2;
using RestWithASPNET10Erudio.Model;

namespace RestWithASPNET10Erudio.Tests.Converter
{
    public class PersonConverterTests
    {
        private readonly PersonConverter _converter;

        public PersonConverterTests()
        {
            _converter = new PersonConverter();
        }

        // ----------------- PersonDTO → Person -----------------
        [Fact]
        public void Parse_ShouldConvertPersonDTOToPerson()
        {
            var dto = new PersonDTO
            {
                Id = 1,
                FirstName = "Leandro",
                LastName = "CG",
                Address = "Rua X",
                Gender = "M",
                BirthDay = new DateTime(1990, 1, 1)
            };

            var person = _converter.Parse(dto);

            person.Should().NotBeNull();
            person.Id.Should().Be(dto.Id);
            person.FirstName.Should().Be(dto.FirstName);
            person.LastName.Should().Be(dto.LastName);
            person.Address.Should().Be(dto.Address);
            person.Gender.Should().Be(dto.Gender);
            person.BirthDay.Should().Be(dto.BirthDay);
        }

        [Fact]
        public void Parse_NullPersonDTO_ShouldReturnNull()
        {
            PersonDTO dto = null;
            var person = _converter.Parse(dto);
            person.Should().BeNull();
        }

        // ----------------- Person → PersonDTO -----------------
        [Fact]
        public void Parse_ShouldConvertPersonToPersonDTO()
        {
            var person = new Person
            {
                Id = 1,
                FirstName = "Leandro",
                LastName = "CG",
                Address = "Rua X",
                Gender = "M",
                BirthDay = new DateTime(1990, 1, 1)
            };

            var dto = _converter.Parse(person);

            dto.Should().NotBeNull();
            dto.Id.Should().Be(person.Id);
            dto.FirstName.Should().Be(person.FirstName);
            dto.LastName.Should().Be(person.LastName);
            dto.Address.Should().Be(person.Address);
            dto.Gender.Should().Be(person.Gender);
            dto.BirthDay.Should().Be(person.BirthDay);
        }

        [Fact]
        public void Parse_NullPerson_ShouldReturnNull()
        {
            Person person = null;
            var dto = _converter.Parse(person);
            dto.Should().BeNull();
        }

        [Fact]
        public void Parse_PersonWithNullBirthDay_ShouldSetBirthDayToNow()
        {
            var person = new Person
            {
                Id = 1,
                FirstName = "Leandro",
                LastName = "CG",
                Address = "Rua X",
                Gender = "M",
                BirthDay = null
            };

            var dto = _converter.Parse(person);

            dto.BirthDay.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        // ----------------- List<PersonDTO> → List<Person> -----------------
        [Fact]
        public void ParseList_ShouldConvertPersonDTOListToPersonList()
        {
            var listDto = new List<PersonDTO>
            {
                new PersonDTO { Id = 1, FirstName = "Leandro" },
                new PersonDTO { Id = 2, FirstName = "Maria" }
            };

            var listPerson = _converter.ParseList(listDto);

            listPerson.Should().HaveCount(2);
            listPerson[0].FirstName.Should().Be("Leandro");
            listPerson[1].FirstName.Should().Be("Maria");
        }

        [Fact]
        public void ParseList_NullList_ShouldReturnNull()
        {
            List<PersonDTO> listDto = null;
            var listPerson = _converter.ParseList(listDto);
            listPerson.Should().BeNull();
        }

        // ----------------- List<Person> → List<PersonDTO> -----------------
        [Fact]
        public void ParseList_ShouldConvertPersonListToPersonDTOList()
        {
            var listPerson = new List<Person>
            {
                new Person { Id = 1, FirstName = "Leandro" },
                new Person { Id = 2, FirstName = "Maria" }
            };

            var listDto = _converter.ParseList(listPerson);

            listDto.Should().HaveCount(2);
            listDto[0].FirstName.Should().Be("Leandro");
            listDto[1].FirstName.Should().Be("Maria");
        }

        [Fact]
        public void ParseList_NullPersonList_ShouldReturnNull()
        {
            List<Person> listPerson = null;
            var listDto = _converter.ParseList(listPerson);
            listDto.Should().BeNull();
        }
    }
}
