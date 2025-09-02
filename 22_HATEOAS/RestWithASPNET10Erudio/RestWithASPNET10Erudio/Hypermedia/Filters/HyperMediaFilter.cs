using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RestWithASPNET10Erudio.Hypermedia.Filters
{
    public class HyperMediaFilter(
        HyperMediaFilterOptions hyperMediaFilterOptions)
        : ResultFilterAttribute
    {
        private readonly HyperMediaFilterOptions _hyperMediaFilterOptions = hyperMediaFilterOptions;
        private readonly ILogger<HyperMediaFilter> _logger;

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            TryEnrichResult(context);
            base.OnResultExecuting(context);
        }

        private void TryEnrichResult(ResultExecutingContext context)
        {
            if (context.Result is OkObjectResult objectResult)
            {
                _logger.LogInformation("Applying HyperMediaFilter for result type {Type}", objectResult.Value?.GetType().Name);

                var enricher = _hyperMediaFilterOptions
                    .ContentResponseEnricherList
                    .FirstOrDefault(option => option.CanEnrich(context));
                if (enricher != null) Task.FromResult(enricher.Enrich(context));
            };
        }
    }
}
