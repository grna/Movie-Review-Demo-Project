namespace SciFiReviewsApi.Helpers
{
    public class MoviesResourceParameters
    {
        const int maxPageSize = 30;

        public int _pageSize = 10;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        public string MovieGenre { get; set; }

        public string SortBy { get; set; } = "submittime";

        public string SearchWord { get; set; }
    }
}
