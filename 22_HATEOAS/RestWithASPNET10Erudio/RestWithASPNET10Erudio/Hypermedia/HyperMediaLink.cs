namespace RestWithASPNET10Erudio.Hypermedia
{
    public class HyperMediaLink
    {
        private string _href;
        private readonly object _lock = new object();

        public string Rel { get; set; }

        public string Href
        {
            get => ThreathHref();
            set => _href = value;
        }

        public string Type { get; set; }
        public string Action { get; set; }

        private string ThreathHref()
        {
            lock (_lock) return _href?.Replace("%2F", "/");
        }
    }
}
