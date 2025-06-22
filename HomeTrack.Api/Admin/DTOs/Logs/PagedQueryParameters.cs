namespace HomeTrack.Api.Request
{
  public class PagedQueryParameters
  {
     private const int MaxPageSize = 50; // Giới hạn kích thước trang tối đa
        private int _pageNumber = 1;
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = (value > 0) ? value : 1;
        }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : (value > 0 ? value : 10);
        }

        public string? SortBy { get; set; } // Tên trường để sắp xếp
        public string? SortOrder { get; set; } = "desc"; // "asc" hoặc "desc"
  }
}