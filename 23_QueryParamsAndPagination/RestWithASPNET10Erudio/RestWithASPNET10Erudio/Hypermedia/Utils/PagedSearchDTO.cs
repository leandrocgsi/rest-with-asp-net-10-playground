using RestWithASPNET10Erudio.Hypermedia.Abstract;

namespace RestWithASPNET10Erudio.Hypermedia.Utils
{
    public class PagedSearchDTO<T> where T : ISupportsHyperMedia
    {
        public int CurrentPage { get; init; }
        public int PageSize { get; init; }
        public int TotalResults { get; set; }
        public string SortFields { get; init; }
        public string SortDirections { get; init; }
        public Dictionary<string, object> Filters { get; init; } = [];
        public List<T> List { get; set; } = [];

        public PagedSearchDTO() { }

        public PagedSearchDTO(
            int currentPage,
            int pageSize,
            string sortFields,
            string sortDirections,
            Dictionary<string, object> filters = null)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            SortFields = sortFields;
            SortDirections = sortDirections;
            Filters = filters ?? [];
        }

        public PagedSearchDTO(
            int currentPage,
            string sortFields,
            string sortDirections)
            : this(currentPage, 10, sortFields, sortDirections) { }

        public int GetCurrentPage() => CurrentPage == 0 ? 1 : CurrentPage;
        public int GetPageSize() => PageSize == 0 ? 10 : PageSize;
    }
}
