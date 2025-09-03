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
            // Obter a base da requisição para URLs absolutas
            var request = urlHelper.ActionContext.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            // Gerar URLs absolutas com IUrlHelper.Action
            var collectionUrl = urlHelper.Action("Get", "Person", null, request.Scheme, request.Host.Host);
            var selfUrl = urlHelper.Action("Get", "Person", new { id = content.Id }, request.Scheme, request.Host.Host);
            var createUrl = urlHelper.Action("Post", "Person", null, request.Scheme, request.Host.Host);
            var updateUrl = urlHelper.Action("Put", "Person", null, request.Scheme, request.Host.Host);
            var patchUrl = urlHelper.Action("Disable", "Person", new { id = content.Id }, request.Scheme, request.Host.Host);
            var deleteUrl = urlHelper.Action("Delete", "Person", new { id = content.Id }, request.Scheme, request.Host.Host);

            // Log para depuração
            //_logger.LogInformation("URLs geradas: collection={Collection}, self={Self}, create={Create}, update={Update}, patch={Patch}, delete={Delete}",
            //    collectionUrl, selfUrl, createUrl, updateUrl, patchUrl, deleteUrl);

            // Verificar se alguma URL é nula
            if (string.IsNullOrEmpty(collectionUrl) || string.IsNullOrEmpty(selfUrl))
            {
                // _logger.LogWarning("Uma ou mais URLs são nulas. Pulando enriquecimento para PersonDTO Id={Id}", content.Id);
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
                Rel = RelationType.Update,
                Type = ResponseTypeFormat.DefaultPatch
            });

            content.Links.Add(new HyperMediaLink
            {
                Action = HttpActionVerb.DELETE,
                Href = deleteUrl,
                Rel = RelationType.Delete,
                Type = ResponseTypeFormat.DefaultDelete
            });

            return Task.CompletedTask;
        }
    }
}
