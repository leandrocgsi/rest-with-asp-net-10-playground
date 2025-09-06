using Microsoft.AspNetCore.Mvc;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Hypermedia.Constants;

namespace RestWithASPNET10Erudio.Hypermedia.Enricher
{
    public class PersonEnricher : ContentResponseEnricher<PersonDTO>
    {
        protected override Task EnrichModel(PersonDTO content, IUrlHelper urlHelper)
        {
            var request = urlHelper.ActionContext.HttpContext.Request;

            var baseUrl = $"{request.Scheme}://{request.Host}/api/person/v1";

            content.Links.AddRange(GenerateLinks(content.Id, baseUrl));

            return Task.CompletedTask;
        }

        private IEnumerable<HyperMediaLink> GenerateLinks(long id, string baseUrl)
        {
            return new List<HyperMediaLink>
            {
                new HyperMediaLink
                {
                    Rel = RelationType.Collection,
                    Href = baseUrl,
                    Type = ResponseTypeFormat.DefaultGet,
                    Action = HttpActionVerb.GET
                },
                new HyperMediaLink
                {
                    Rel = RelationType.Self,
                    Href = $"{baseUrl}/{id}",
                    Type = ResponseTypeFormat.DefaultGet,
                    Action = HttpActionVerb.GET
                },
                new HyperMediaLink
                {
                    Rel = RelationType.Create,
                    Href = baseUrl,
                    Type = ResponseTypeFormat.DefaultPost,
                    Action = HttpActionVerb.POST
                },
                new HyperMediaLink
                {
                    Rel = RelationType.Update,
                    Href = baseUrl,
                    Type = ResponseTypeFormat.DefaultPut,
                    Action = HttpActionVerb.PUT
                },
                new HyperMediaLink
                {
                    Rel = RelationType.Update,
                    Href = $"{baseUrl}/{id}",
                    Type = ResponseTypeFormat.DefaultPatch,
                    Action = HttpActionVerb.PATCH
                },
                new HyperMediaLink
                {
                    Rel = RelationType.Delete,
                    Href = $"{baseUrl}/{id}",
                    Type = ResponseTypeFormat.DefaultDelete,
                    Action = HttpActionVerb.DELETE
                }
            };
        }
    }
}
