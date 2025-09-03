using System.Xml.Serialization;

namespace RestWithASPNET10Erudio.Hypermedia
{
    public class HyperMediaLink
    {
        // private string _href = string.Empty;

        [XmlAttribute]
        public string Rel { get; set; } = string.Empty;

        [XmlAttribute]
        public string Href
        {
            get; // => _href?.Replace("%2F", "/") ?? string.Empty;
            set; // => _href = value ?? string.Empty;
        }

        [XmlAttribute]
        public string Type { get; set; } = "application/json";

        [XmlAttribute]
        public string Action { get; set; } = string.Empty;
    }
}
