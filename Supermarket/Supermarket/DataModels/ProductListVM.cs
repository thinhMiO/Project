using Supermarket.Models;

namespace Supermarket.DataModels
{
    public class ProductListVM
    {
        public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
        public PagingInfo PagingInfo { get; set; } = new PagingInfo();
    }
}
