namespace Supermarket.DataModels
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurentPage { get; set; }
        public int TotalPage =>(int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
    }
}
