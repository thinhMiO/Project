namespace Supermarket.Areas.Admin.Data
{
    public class ReviewVM
    {
        public int ReviewId { get; set; }
        public string? CustomerName{ get; set; }
        public string? ProductName { get; set; }
        public string? ShortContents { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
