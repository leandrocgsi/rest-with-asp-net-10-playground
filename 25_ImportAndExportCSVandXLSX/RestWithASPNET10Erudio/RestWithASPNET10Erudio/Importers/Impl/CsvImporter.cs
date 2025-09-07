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
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true
            });

            var records = csv.GetRecords<PersonDTO>();
            return await Task.FromResult(records.ToList());
        }
    }
}
