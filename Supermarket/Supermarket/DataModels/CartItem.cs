namespace Supermarket.DataModels
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public double? Price { get; set; }
        public double? Discount { get; set; }
        public int Quantity { get; set; }
        public string? Image { get; set; }
        public double Total => (double)(Quantity * Price * (1 - Discount / 100));
    }
}
