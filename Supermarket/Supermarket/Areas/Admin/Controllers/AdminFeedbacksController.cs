using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Supermarket.Models;

namespace Supermarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminFeedbacksController : Controller
    {
        private readonly ShopContext _context;

        public AdminFeedbacksController(ShopContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminFeedbacks
        public async Task<IActionResult> Index()
        {
              return _context.Feedbacks != null ? 
                          View(await _context.Feedbacks.ToListAsync()) :
                          Problem("Entity set 'ShopContext.Feedbacks'  is null.");
        }

        // GET: Admin/AdminFeedbacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Feedbacks == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        private bool FeedbackExists(int id)
        {
          return (_context.Feedbacks?.Any(e => e.FeedbackId == id)).GetValueOrDefault();
        }
    }
}
