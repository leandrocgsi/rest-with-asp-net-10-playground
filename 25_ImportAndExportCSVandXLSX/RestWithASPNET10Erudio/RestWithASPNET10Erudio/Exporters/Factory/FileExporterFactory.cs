using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestWithASPNET10Erudio.Exporters.Contract;
using RestWithASPNET10Erudio.Exporters.Impl;
using RestWithASPNET10Erudio.Importers.Impl;

namespace RestWithASPNET10Erudio.Exporters.Factory
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
