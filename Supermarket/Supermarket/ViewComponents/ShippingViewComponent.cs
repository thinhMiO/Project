using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supermarket.Models;

namespace Supermarket.ViewComponents
{
    public class ShippingViewComponent : ViewComponent
    {
        private readonly ShopContext _context;

        public ShippingViewComponent(ShopContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var customerIdClaim = HttpContext.User.FindFirst("CustomerID");
            if (customerIdClaim != null)
            {
                int customerId = int.Parse(customerIdClaim.Value);
                var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
                if (customer != null)
                {
                    return View("ShippingOrder", _context.Orders.Where(o => o.CustomerId == customerId).Where(o => o.TransactStatusId == 3).Include(o => o.TransactStatus).ToList());
                }
            }

            return View();
        }
    }
}
