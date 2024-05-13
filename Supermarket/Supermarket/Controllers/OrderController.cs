using Microsoft.AspNetCore.Mvc;
using Supermarket.Models;

namespace Supermarket.Controllers
{
    public class OrderController : Controller
    {
        private readonly ShopContext _context;

        public OrderController(ShopContext context)
        {
            _context = context;
        }

        [HttpDelete]
        [Route("/order/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
                if (order == null)
                {
                    return NotFound(); 
                }
                order.TransactStatusId = 1;
                _context.SaveChanges();
                var items = _context.OrderDetails.Where(o => o.OrderId == id).ToList();
                foreach (var item in items)
                {
                    var pros = _context.Products.Where(p => p.ProductId == item.ProductId).FirstOrDefault();

                    pros.Quantity = pros.Quantity + item.Quantity;
                    _context.SaveChanges();
                }

                
                return Ok();
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, ex.Message);
            }
        }
    }
}
