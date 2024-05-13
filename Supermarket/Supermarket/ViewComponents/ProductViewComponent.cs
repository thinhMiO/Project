using Microsoft.AspNetCore.Mvc;
using Supermarket.Models;

namespace Supermarket.ViewComponents
{
    public class ProductViewComponent :ViewComponent
    {
        private readonly ShopContext _context;
        public ProductViewComponent(ShopContext context) 
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View(_context.Products.Where(p => p.BestSeller == true).ToList());
        }
    }
}
