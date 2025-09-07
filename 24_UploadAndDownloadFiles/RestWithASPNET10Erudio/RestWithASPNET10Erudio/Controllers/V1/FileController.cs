using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Services;

namespace RestWithASPNET10Erudio.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]/v1")]
    public class FileController(IFileServices fileServices)
        : Controller
    {

        private readonly IFileServices _fileServices = fileServices;

        [HttpGet("downloadFile/{fileName}")]
        [ProducesResponseType(200, Type = typeof(byte[]))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Produces("application/octet-stream")]
        public IActionResult GetFileAsync(string fileName)
        {
            var buffer = _fileServices.GetFile(fileName);
            if (buffer == null || buffer.Length == 0)
                return NoContent();

            var contentType = $"application/{Path.GetExtension(fileName).TrimStart('.')}";
            return File(buffer, contentType, fileName);
        }

        [HttpPost("uploadFile")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200, Type = typeof(FileDetailDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Produces("application/json")]
        // public async Task<IActionResult> UploadOneFile([FromForm(Name = "file")] IFormFile file)
        public async Task<IActionResult> UploadOneFile([FromForm] FileUploadDTO input)
        {
            var detail = await _fileServices.SaveFileToDisk(input.File);
            return Ok(detail);
        }

        [HttpPost("uploadMultipleFiles")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(200, Type = typeof(List<FileDetailDTO>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Produces("application/json")]
        // public async Task<IActionResult> UploadManyFiles([FromForm(Name = "files")] List<IFormFile> files)
        public async Task<IActionResult> UploadMultipleFiles([FromForm] MultipleFilesUploadDTO input)

        {
            var details = await _fileServices.SaveFilesToDisk(input.Files);
            return Ok(details);
        }
    }
}
