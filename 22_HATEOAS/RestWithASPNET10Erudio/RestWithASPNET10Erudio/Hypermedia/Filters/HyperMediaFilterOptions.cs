using RestWithASPNET10Erudio.Hypermedia.Abstract;

namespace RestWithASPNET10Erudio.Hypermedia.Filters
{
    public class HyperMediaFilterOptions
    {
        public List<IResponseEnricher> ContentResponseEnricherList { get; set; } = [];
    }
}
