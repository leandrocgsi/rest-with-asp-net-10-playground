using Microsoft.AspNetCore.Mvc;
using RestWithASPNET10Erudio.Data.DTO.V1;

namespace RestWithASPNET10Erudio.Exporters.Contract
{
    public interface IFileExporter
    {
        FileContentResult ExportFile(List<PersonDTO> people);
    }
}