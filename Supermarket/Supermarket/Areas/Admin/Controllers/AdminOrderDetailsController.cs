using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Supermarket.Models;

namespace Supermarket.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AdminOrderDetailsController : Controller
    {
        private readonly ShopContext _context;

        public AdminOrderDetailsController(ShopContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminOrderDetails
        public async Task<IActionResult> Index(int? OrDerId = 0 )
        {
            OrDerId = OrDerId ?? 0;
            var shopContext = _context.OrderDetails.Where(o => o.OrderId == OrDerId).Include(o => o.Order).Include(o => o.Product);
            return View(await shopContext.ToListAsync());
        }

        private bool OrderDetailExists(int id)
        {
          return (_context.OrderDetails?.Any(e => e.OrderDetailId == id)).GetValueOrDefault();
        }
    }
}
