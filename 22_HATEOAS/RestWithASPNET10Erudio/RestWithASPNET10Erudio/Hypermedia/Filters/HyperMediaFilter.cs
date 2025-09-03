using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RestWithASPNET10Erudio.Hypermedia.Filters
{
    public class HyperMediaFilter : ResultFilterAttribute
    {
        private readonly HyperMediaFilterOptions _hyperMediaFilterOptions;
        private readonly ILogger _logger;

        public HyperMediaFilter(
          HyperMediaFilterOptions hyperMediaFilterOptions,
          ILogger<HyperMediaFilter> logger)
        {
            _hyperMediaFilterOptions = hyperMediaFilterOptions ??
              throw new ArgumentNullException(nameof(hyperMediaFilterOptions));
            _logger = logger ??
              throw new ArgumentNullException(nameof(logger));
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            TryEnrichResult(context);
            base.OnResultExecuting(context);
        }

        private void TryEnrichResult(ResultExecutingContext context)
        {
            if (context.Result is OkObjectResult objectResult)
            {
                _logger.LogInformation("Aplicando HyperMediaFilter para tipo de resultado {Type}", objectResult.Value?.GetType().Name);

                var enricher = _hyperMediaFilterOptions
                  .ContentResponseEnricherList
                  .FirstOrDefault(option => option.CanEnrich(context));

                if (enricher != null)
                {
                    enricher.Enrich(context).Wait(); // Executa sincronicamente
                }
            }
        }
    }

}