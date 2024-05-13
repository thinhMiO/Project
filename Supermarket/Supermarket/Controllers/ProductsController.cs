using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Supermarket.DataModels;
using Supermarket.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Supermarket.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ShopContext _context;
        public int PageSize = 9;

        public ProductsController(ShopContext context)
        {
            _context = context;
        }
        public class PriceRange
        {
            public float Min { get; set; }
            public float Max { get; set; }
        }
        public IActionResult GetFilteredProducts([FromBody] FilterData filter)
        {
            var filterproducts = _context.Products.Include(o => o.Category).ToList();
            if(filter.PriceRange != null && filter.PriceRange.Count > 0 && !filter.PriceRange.Contains("all"))
            {
                List<PriceRange> priceRanges = new List<PriceRange>();
                foreach(var range in filter.PriceRange)
                {
                    var values = range.Split("-").ToArray();
                    PriceRange priceRange = new PriceRange();
                    priceRange.Min = float.Parse(values[0]);
                    priceRange.Max = float.Parse(values[1]);
                    priceRanges.Add(priceRange);
                }
                filterproducts = filterproducts.Where(p => priceRanges.Any(r =>p.Price >= r.Min && p.Price <= r.Max)).ToList();
            }
            if (filter.Categories != null && filter.Categories.Count > 0 && !filter.Categories.Contains("all"))
            {
                filterproducts = filterproducts.Where(p=> filter.Categories.Contains(p.Category.CategoryName)).ToList();
            }
                return PartialView("_ReturnProducts",filterproducts);
        }

        // GET: Products
        public IActionResult Index(int productPage = 1)
        {
            var product = _context.Products.Skip((productPage-1)*PageSize).Take(PageSize);


            return View(
                    new ProductListVM
                    {
                        Products = _context.Products
                        .Skip((productPage - 1) * PageSize)
                        .Take(PageSize),
                        PagingInfo = new PagingInfo
                        {
                            ItemsPerPage = PageSize,
                            CurentPage = productPage,
                            TotalItems = _context.Products.Count()
                        }
                    }
                );
        }
        [HttpPost]
        public IActionResult Search(string keyword, int productPage = 1)
        {
            var product = _context.Products.Skip((productPage - 1) * PageSize).Take(PageSize);


            return View("Index",
                    new ProductListVM
                    {
                        Products = _context.Products
                        .Where(p => p.ProductName.Contains(keyword))
                        .Skip((productPage - 1) * PageSize)
                        .Take(PageSize),
                        PagingInfo = new PagingInfo
                        {
                            ItemsPerPage = PageSize,
                            CurentPage = productPage,
                            TotalItems = _context.Products.Count()
                        }
                    }
                );
        }
        public IActionResult ProductByCat(int categoryId, int productPage = 1)
        {
            var product = _context.Products.Where(p => p.CategoryId == categoryId);
            product = _context.Products.Skip((productPage - 1) * PageSize).Take(PageSize);
            return View("Index",
                    new ProductListVM
                    {
                        Products = _context.Products
                        .Where(p => p.CategoryId == categoryId)
                        .Skip((productPage - 1) * PageSize)
                        .Take(PageSize),
                        PagingInfo = new PagingInfo
                        {
                            ItemsPerPage = PageSize,
                            CurentPage = productPage,
                            TotalItems = _context.Products.Count()
                        }
                    }
                );
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var customerId = int.Parse(HttpContext.User.Claims.SingleOrDefault(p => p.Type == "CustomerID").Value);
                if (customerId == null)
                {
                    return NotFound();
                }
                var cus = _context.Reviews.Where(r => r.CustomerId == customerId && r.ProductId == id).Count();
                ViewBag.Cus = cus;
                ViewBag.ID = customerId;
            }
            
            var reviews = _context.Reviews.Where(r => r.ProductId == id).Include(c => c.Customer).ToList();
            var reviewcount = _context.Reviews.Where(r => r.ProductId == id).Count();
            ViewBag.Pros = _context.Products.Where(p => p.ProductId != id).ToList();
           
            
            
            ViewBag.Reviews = reviews;
            ViewBag.RVCount = reviewcount;
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
