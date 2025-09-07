using ClosedXML.Excel;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Importers.Contract;

namespace RestWithASPNET10Erudio.Importers.Impl
{
    public class XlsxImporter : IFileImporter
    {
        public async Task<List<PersonDTO>> ImportFileAsync(Stream fileStream)
        {
            var persons = new List<PersonDTO>();

            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RowsUsed().Skip(1); // pula cabeçalho

            foreach (var row in rows)
            {
                // valida se a linha tem dados antes de tentar converter
                if (!row.Cell(1).IsEmpty())
                {
                    persons.Add(new PersonDTO
                    {
                        FirstName = row.Cell(1).GetString(),
                        LastName = row.Cell(2).GetString(),
                        Address = row.Cell(3).GetString(),
                        Gender = row.Cell(4).GetString(),
                        Enabled = true
                    });
                }
            }

            return await Task.FromResult(persons);
        }
    }
}