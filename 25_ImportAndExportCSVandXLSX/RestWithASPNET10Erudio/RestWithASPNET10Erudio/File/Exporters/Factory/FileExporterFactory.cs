using RestWithASPNET10Erudio.File.Exporters.Contract;
using RestWithASPNET10Erudio.File.Exporters.Impl;

namespace RestWithASPNET10Erudio.File.Exporters.Factory
{
    public class FileExporterFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FileExporterFactory> _logger;

        public FileExporterFactory(IServiceProvider serviceProvider, ILogger<FileExporterFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public IFileExporter GetExporter(string acceptHeader)
        {
            if (string.Equals(acceptHeader, MediaTypes.ApplicationXlsx, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Using XlsxExporter for Accept: {AcceptHeader}", acceptHeader);
                return _serviceProvider.GetRequiredService<XlsxExporter>();
            }
            else if (string.Equals(acceptHeader, MediaTypes.ApplicationCsv, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Using CsvExporter for Accept: {AcceptHeader}", acceptHeader);
                return _serviceProvider.GetRequiredService<CsvExporter>();
            }
            else
            {
                throw new NotSupportedException("Invalid format for export.");
            }
        }
    }

}
