using Microsoft.AspNetCore.Mvc;
using Supermarket.Models;
using System.Data.Entity;

namespace Supermarket.ViewComponents
{
    public class PendingViewComponent : ViewComponent
    {
        private readonly ShopContext _context;

        public PendingViewComponent(ShopContext context)
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
                    return View("PendingOrder", _context.Orders.Where(o => o.CustomerId == customerId).Where(o => o.TransactStatusId == 2)
                        .Include(o => o.TransactStatus).OrderByDescending(o=>o.OrderDate).ToList());
                }
            }

            return View();
        }
    }
}
