using Microsoft.AspNetCore.Mvc;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Hypermedia.Constants;
using RestWithASPNET10Erudio.Hypermedia.Utils;

namespace RestWithASPNET10Erudio.Hypermedia.Enricher
{
    public class BookEnricher : ContentResponseEnricher<BookDTO>
    {
        protected override Task EnrichModel(BookDTO content, IUrlHelper urlHelper)
        {
            var baseUrl = urlHelper.BuildBaseUrl("DefaultApi", "api/person");

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
                Action = HttpActionVerb.DELETE,
                Href = $"{baseUrl}/{content.Id}",
                Rel = RelationType.Delete,
                Type = "int"
            });

            return Task.CompletedTask;
        }
    }
}