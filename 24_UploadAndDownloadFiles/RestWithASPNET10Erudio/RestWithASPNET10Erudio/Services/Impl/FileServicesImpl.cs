using RestWithASPNET10Erudio.Data.DTO.V1;

namespace RestWithASPNET10Erudio.Services.Impl
{
    public class FileServicesImpl : IFileServices
    {

        private readonly string _basePath;
        private readonly IHttpContextAccessor _context;

        private static readonly HashSet<string> _allowedExtensions =
            new(StringComparer.OrdinalIgnoreCase)
            { ".pdf", ".jpg", ".jpeg", ".png", ".txt", ".docx", ".mp3" };

        public FileServicesImpl(IHttpContextAccessor context)
        {
            _context = context;
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadDir");
            Directory.CreateDirectory(_basePath); // garante que a pasta exista
        }

        public byte[] GetFile(string fileName)
        {
            var filePath = Path.Combine(_basePath, fileName);
            return File.Exists(filePath) ? File.ReadAllBytes(filePath) : Array.Empty<byte>();
        }

        public async Task<FileDetailDTO> SaveFileToDisk(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new FileDetailDTO();

            var fileType = Path.GetExtension(file.FileName);
            if (!_allowedExtensions.Contains(fileType))
                return new FileDetailDTO();

            var docName = Path.GetFileName(file.FileName);
            var destination = Path.Combine(_basePath, docName);

            var baseUrl = $"{_context.HttpContext?.Request.Scheme}://{_context.HttpContext?.Request.Host}";

            var fileDetail = new FileDetailDTO
            {
                DocumentName = docName,
                DocType = fileType,
                DocUrl = $"{baseUrl}/api/file/v1/downloadFile/{docName}"
            };

            using var stream = new FileStream(destination, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileDetail;
        }

        public async Task<List<FileDetailDTO>> SaveFilesToDisk(IList<IFormFile> files)
        {
            var results = new List<FileDetailDTO>();
            foreach (var file in files)
            {
                var detail = await SaveFileToDisk(file);
                if (!string.IsNullOrEmpty(detail.DocumentName)) // só adiciona se válido
                    results.Add(detail);
            }
            return results;
        }
    }
}
