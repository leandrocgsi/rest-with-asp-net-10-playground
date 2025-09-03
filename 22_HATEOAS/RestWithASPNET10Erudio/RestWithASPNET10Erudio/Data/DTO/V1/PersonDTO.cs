using RestWithASPNET10Erudio.Hypermedia;
using RestWithASPNET10Erudio.Hypermedia.Abstract;
using System.Xml.Serialization;

namespace RestWithASPNET10Erudio.Data.DTO.V1
{
    public class PersonDTO : ISupportsHyperMedia
    {
        [XmlElement("id")]
        public long Id { get; set; }

        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("address")]
        public string Address { get; set; }

        [XmlElement("gender")]
        public string Gender { get; set; }

        [XmlElement("enabled")]
        public bool Enabled { get; set; }

        [XmlElement("link")]
        public List<HyperMediaLink> Links { get; set; } = [];

    }
}
