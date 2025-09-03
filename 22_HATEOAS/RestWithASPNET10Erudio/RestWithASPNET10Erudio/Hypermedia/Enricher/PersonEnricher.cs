using Microsoft.AspNetCore.Mvc;
using RestWithASPNET10Erudio.Data.DTO.V1;
using RestWithASPNET10Erudio.Hypermedia.Constants;

namespace RestWithASPNET10Erudio.Hypermedia.Enricher
{
    public class PersonEnricher : ContentResponseEnricher<PersonDTO>
    {
        //private readonly ILogger<PersonEnricher> _logger;

        // public PersonEnricher(ILogger<PersonEnricher> logger)
        public PersonEnricher()
        {
            //_logger = logger;
        }

        protected override Task EnrichModel(PersonDTO content, IUrlHelper urlHelper)
        {
            // _logger.LogInformation(...); // Mantenha se necessário

            var collectionUrl = urlHelper.Action("Get", "Person"); // Para lista: api/person/v1
            var selfUrl = urlHelper.Action("Get", "Person", new { id = content.Id }); // api/person/v1/{id}
            var createUrl = urlHelper.Action("Post", "Person"); // api/person/v1
            var updateUrl = urlHelper.Action("Put", "Person"); // api/person/v1 (corpo inclui ID)
            var patchUrl = urlHelper.Action("Disable", "Person", new { id = content.Id }); // api/person/v1/{id} (nome da ação é "Disable" para Patch)
            var deleteUrl = urlHelper.Action("Delete", "Person", new { id = content.Id }); // api/person/v1/{id}

            // Fallback se a URL principal não for gerada (ex.: roteamento mal configurado)
            if (string.IsNullOrEmpty(collectionUrl))
            {
                // _logger.LogWarning("Não foi possível gerar collectionUrl para PersonDTO {Id}", content.Id);
                return Task.CompletedTask;
            }

            content.Links.Add(new HyperMediaLink
            {
                Action = HttpActionVerb.GET,
                Href = collectionUrl,
                Rel = RelationType.Collection,
                Type = ResponseTypeFormat.DefaultGet
            });

            content.Links.Add(new HyperMediaLink
            {
                Action = HttpActionVerb.GET,
                Href = selfUrl,
                Rel = RelationType.Self,
                Type = ResponseTypeFormat.DefaultGet
            });

            content.Links.Add(new HyperMediaLink
            {
                Action = HttpActionVerb.POST,
                Href = createUrl,
                Rel = RelationType.Create,
                Type = ResponseTypeFormat.DefaultPost
            });

            content.Links.Add(new HyperMediaLink
            {
                Action = HttpActionVerb.PUT,
                Href = updateUrl,
                Rel = RelationType.Update,
                Type = ResponseTypeFormat.DefaultPut
            });

            content.Links.Add(new HyperMediaLink
            {
                Action = HttpActionVerb.PATCH,
                Href = patchUrl,
                Rel = RelationType.Update, // Ou use RelationType.Patch se quiser distinguir
                Type = ResponseTypeFormat.DefaultPatch
            });

            content.Links.Add(new HyperMediaLink
            {
                Action = HttpActionVerb.DELETE,
                Href = deleteUrl,
                Rel = RelationType.Delete,
                Type = ResponseTypeFormat.DefaultDelete
            });

            // _logger.LogDebug(...);
            return Task.CompletedTask;
        }
    }
}