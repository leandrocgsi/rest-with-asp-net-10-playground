using RestWithASPNET10Erudio.Hypermedia;
using RestWithASPNET10Erudio.Hypermedia.Abstract;

namespace RestWithASPNET10Erudio.Data.DTO.V1
{

    public class BookDTO : ISupportsHyperMedia
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public DateTime LaunchDate { get; set; }
        public List<HyperMediaLink> Links { get; set; } = [];
    }
}
