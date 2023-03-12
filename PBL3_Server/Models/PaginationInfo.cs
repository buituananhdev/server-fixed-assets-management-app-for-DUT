namespace PBL3_Server.Models
{
    public class PaginationInfo
    {
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public int PageSize { get; set; }
    }
}
