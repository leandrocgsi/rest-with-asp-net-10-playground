using RestWithASPNET10Erudio.File.Importers.Contract;
using RestWithASPNET10Erudio.File.Importers.Impl;

namespace RestWithASPNET10Erudio.File.Importers.Factory
{
    public class FileImporterFactory(
        IServiceProvider serviceProvider,
        ILogger<FileImporterFactory> logger
    )
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<FileImporterFactory> _logger = logger;

        public IFileImporter GetImporter(string fileName)
        {
            if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Using XlsxImporter for file: {FileName}", fileName);
                return _serviceProvider.GetRequiredService<XlsxImporter>();
            }
            else if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Using CsvImporter for file: {FileName}", fileName);
                return _serviceProvider.GetRequiredService<CsvImporter>();
            }
            else
            {
                throw new NotSupportedException("Invalid file format.");
            }
        }
    }
}
