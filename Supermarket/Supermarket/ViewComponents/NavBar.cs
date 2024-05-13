using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Supermarket.DataModels;
using Supermarket.Models;
using Supermarket.Util;


namespace Supermarket.ViewComponents
{
    public class NavBar :ViewComponent
    {
        private readonly ShopContext _context;

        public NavBar(ShopContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View(_context.Categories.ToList());
        }
    }
}
