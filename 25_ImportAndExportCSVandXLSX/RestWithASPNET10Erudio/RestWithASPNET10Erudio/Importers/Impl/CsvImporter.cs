using CsvHelper;
using CsvHelper.Configuration;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Importers.Contract;
using System.Globalization;

namespace RestWithASPNET10Erudio.Importers.Impl
{
    public class CsvImporter : IFileImporter
    {
        public async Task<List<PersonDTO>> ImportFileAsync(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,        // considera a primeira linha como cabeçalho
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true
            });

            var records = new List<PersonDTO>();

            await foreach (var record in csv.GetRecordsAsync<PersonDTO>())
            {
                // garante que os dados sejam válidos e cria a entidade
                records.Add(new PersonDTO
                {
                    FirstName = record.FirstName,
                    LastName = record.LastName,
                    Address = record.Address,
                    Gender = record.Gender,
                    Enabled = true
                });
            }

            return records;
        }
    }
}