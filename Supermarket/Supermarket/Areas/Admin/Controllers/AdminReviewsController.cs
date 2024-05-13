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
using Supermarket.Areas.Admin.Data;

namespace Supermarket.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AdminReviewsController : Controller
    {
        private readonly ShopContext _context;

        public AdminReviewsController(ShopContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminReviews
        //public async Task<IActionResult> Index()
        //{
        //    var shopContext = _context.Reviews.Include(r => r.Customer).Include(r => r.Product);
        //    return View(await shopContext.ToListAsync());
        //}
        public ActionResult Index()
        {
            var reviews = _context.Reviews
                    .Include(r => r.Customer)
                    .Include(r => r.Product)
                    .Select(r => new ReviewVM
                    {
                        ReviewId = r.ReviewId,
                        CustomerName = r.Customer.CustomerName,
                        ProductName = r.Product.ProductName,
                        ShortContents = (r.Contents != null && r.Contents.Length > 20) ? r.Contents.Substring(0, 20) : r.Contents,
                        Status = r.Status,
                        CreateDate = r.CreateDate,
                    })
                    .ToList();
            return View(reviews);
        }
        // GET: Admin/AdminReviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }


        // GET: Admin/AdminReviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", review.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", review.ProductId);
            return View(review);
        }

        // POST: Admin/AdminReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,CustomerId,ProductId,Contents,Status,CreateDate")] Review review)
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", review.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", review.ProductId);
            if (id != review.ReviewId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(review);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(review.ReviewId))
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
        private bool ReviewExists(int id)
        {
          return (_context.Reviews?.Any(e => e.ReviewId == id)).GetValueOrDefault();
        }

    }
}
