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
using X.PagedList;

namespace Supermarket.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly ShopContext _context;

        public AdminOrdersController(ShopContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminOrders
        public IActionResult Index(int page = 1, int? TransactStatusID = 0)
        {
            page = page < 1 ? 1 : page;
            int pageSize = 10;
            TransactStatusID = TransactStatusID ?? 0;
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status",TransactStatusID);
            var shopContext = _context.Orders.Include(o => o.Customer).Include(o => o.TransactStatus)
                              .OrderByDescending(o => o.OrderDate).ToPagedList(page, pageSize);
            if(TransactStatusID != 0)
            {
                shopContext = _context.Orders.Where(o =>o.TransactStatusId == TransactStatusID).Include(o => o.Customer).Include(o => o.TransactStatus)
                              .OrderByDescending(p => p.OrderDate).ToPagedList(page, pageSize);
            }
            else
            {
                shopContext = _context.Orders.Include(o => o.Customer).Include(o => o.TransactStatus).OrderByDescending(p => p.OrderDate)
                              .ToPagedList(page, pageSize);
            }
            return View(shopContext);
        }

        // GET: Admin/AdminOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Admin/AdminOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return View(order);
        }

        // POST: Admin/AdminOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,OrderDate,TransactStatusId,DeliveryDate,CustomerName,Address,Email,Phone,TransportFee,PaymentMethod,TotalAmount")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   if(order.TransactStatusId == 4)
                    {
                        order.DeliveryDate = DateTime.Now;
                    }
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "TransactStatusId", order.TransactStatusId);
            return View(order);
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
