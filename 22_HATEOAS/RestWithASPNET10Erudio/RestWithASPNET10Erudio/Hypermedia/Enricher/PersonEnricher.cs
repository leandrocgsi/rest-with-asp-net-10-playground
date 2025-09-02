using Microsoft.AspNetCore.Mvc;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Hypermedia.Constants;
using RestWithASPNET10Erudio.Hypermedia.Utils;
using System.Text;
using System.Threading;

namespace RestWithASPNET10Erudio.Hypermedia.Enricher
{
    public class PersonEnricher : ContentResponseEnricher<PersonDTO>
    {
        private static readonly object _lock = new object();
        private readonly ILogger<PersonEnricher> _logger;

        protected override Task EnrichModel(PersonDTO content, IUrlHelper urlHelper)
        {
            _logger.LogInformation("Enriching PersonDTO {FirstName} {LastName}", content.FirstName, content.LastName);


            // var baseUrl = urlHelper.BuildBaseUrl("DefaultApi", "api/person");
            // var baseUrl = BuildBaseUrl("DefaultApi", "api/person");
            var path = "api/person";
            var baseUrl = BuildBaseUrl(urlHelper, path);

            content.Links.Add(new HyperMediaLink
            {
                Action = HttpActionVerb.GET,
                Href = baseUrl,
                Rel = RelationType.Collection,
                Type = ResponseTypeFormat.DefaultGet
            });
            content.Links.Add(new HyperMediaLink
            {
                Action = HttpActionVerb.GET,
                Href = $"{baseUrl}/{content.Id}",
                Rel = RelationType.Self,
                Type = ResponseTypeFormat.DefaultGet
            });

            content.Links.Add(new HyperMediaLink
            {
                Action = HttpActionVerb.POST,
                Href = baseUrl,
                Rel = RelationType.Create,
                Type = ResponseTypeFormat.DefaultPost
            });

            content.Links.Add(new HyperMediaLink
            {
                Action = HttpActionVerb.PUT,
                Href = baseUrl,
                Rel = RelationType.Update,
                Type = ResponseTypeFormat.DefaultPut
            });

            content.Links.Add(new HyperMediaLink
            {
                Action = HttpActionVerb.PATCH,
                Href = $"{baseUrl}/{content.Id}",
                Rel = RelationType.Update,
                Type = ResponseTypeFormat.DefaultPatch
            });

            content.Links.Add(new HyperMediaLink
            {
                Action = HttpActionVerb.DELETE,
                Href = $"{baseUrl}/{content.Id}",
                Rel = RelationType.Delete,
                Type = "int"
            });

            _logger.LogDebug("Links added for PersonDTO {FirstName} {LastName}, total links: {Count}",
       content.FirstName, content.LastName, content.Links.Count);

            return Task.CompletedTask;
        }

        private string BuildBaseUrl(IUrlHelper urlHelper, string path)
        {
            lock (_lock)
            {
                var url = new { controller = path };
                return new StringBuilder(urlHelper.Link("DefaultApi", url)).Replace("%2F", "/").ToString();
            };
        }
    }
}