using Microsoft.AspNetCore.Mvc;
using Supermarket.Models;

namespace Supermarket.ViewComponents
{
    public class ProductDiscountViewComponent : ViewComponent
    {
        private readonly ShopContext _context;
        public ProductDiscountViewComponent(ShopContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View("Discount",_context.Products.Where(p => p.Discount > 0).OrderByDescending(p =>p.Discount).Take(8).ToList());
        }
    }
}
