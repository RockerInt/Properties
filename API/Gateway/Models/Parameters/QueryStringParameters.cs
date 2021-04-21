namespace Gateway.Models.Parameters
{
    public abstract class QueryStringParameters
    {
        const int maxPageSize = 50;

        public bool Paging { get; set; } = false;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
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

        public string GetPaginingQueryString() => Paging ? $"Paging=true&PageNumber={PageNumber}&PageSize={PageSize}" : "Paging=false";
    }
}