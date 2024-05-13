using Microsoft.AspNetCore.Mvc;
using Supermarket.DataModels;
using Supermarket.Models;
using Supermarket.Util;

namespace Supermarket.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var count  = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();
            return View( "CartPanel", new CartModel
            {
                Quantity = count.Sum(x => x.Quantity),
                Total = count.Sum(x => x.Total),
            });
        }
    }
}
