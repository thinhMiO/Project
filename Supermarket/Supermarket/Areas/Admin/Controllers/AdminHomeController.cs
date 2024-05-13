using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supermarket.Models;

namespace Supermarket.Areas.Admin.Controllers;

    [Authorize(Roles ="Admin")]
    [Area("Admin")]
    public class AdminHomeController : Controller
    {
        private readonly ShopContext _context;

        public AdminHomeController(ShopContext context)
        {
            _context = context;
        }
        public IActionResult Index()
            {
                ViewBag.Money = _context.Orders.Sum(o=> o.TotalAmount);
                ViewBag.Sales = _context.OrderDetails.Sum(o=>o.Quantity);
                ViewBag.Clients = _context.Customers.Count() -1;
                ViewBag.Orders = _context.Orders.Count();
                return View();
            }
    }
