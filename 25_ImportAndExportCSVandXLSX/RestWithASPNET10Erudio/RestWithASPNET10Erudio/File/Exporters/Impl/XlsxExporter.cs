using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.File.Exporters.Contract;
using RestWithASPNET10Erudio.File.Exporters.Factory;

namespace RestWithASPNET10Erudio.File.Exporters.Impl
{
    public class XlsxExporter : IFileExporter
    {
        public FileContentResult ExportFile(List<PersonDTO> people)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("People");

            // Cabeçalho
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "First Name";
            worksheet.Cell(1, 3).Value = "Last Name";
            worksheet.Cell(1, 4).Value = "Address";
            worksheet.Cell(1, 5).Value = "Gender";
            worksheet.Cell(1, 6).Value = "Enabled";

            var headerRange = worksheet.Range(1, 1, 1, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Preenche os dados
            int row = 2;
            foreach (var person in people)
            {
                worksheet.Cell(row, 1).Value = person.Id;
                worksheet.Cell(row, 2).Value = person.FirstName;
                worksheet.Cell(row, 3).Value = person.LastName;
                worksheet.Cell(row, 4).Value = person.Address;
                worksheet.Cell(row, 5).Value = person.Gender;
                worksheet.Cell(row, 6).Value = person.Enabled == true ? "Yes" : "No";
                row++;
            }

            // Ajusta colunas automaticamente
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var fileBytes = stream.ToArray();

            return new FileContentResult(fileBytes, MediaTypes.ApplicationXlsx)
            {
                FileDownloadName = $"people_exported_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx"
            };
        }
    }
}