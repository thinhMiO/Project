
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using Supermarket.Models;
using Supermarket.Util;
using Microsoft.AspNetCore.Authorization;
using Supermarket.DataModels;
using System.Security.Claims;
using System.Net;
using Supermarket.Extension;

namespace Supermarket.Controllers
{
    public class AddToCartController : Controller
    {
        private readonly ShopContext _context;
        private readonly PaypalClient _paypalClient;

        public AddToCartController(ShopContext context,PaypalClient paypalClient)
        {
            _context = context;
            _paypalClient = paypalClient;
        }
        public List<CartItem> Carts => HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>(); 
        public IActionResult MyOrder()
        {
            return View(Carts);
        }
        public IActionResult AddToCart(int id,int quantity = 1, string returnUrl = null)
        {
            var myCart = Carts;
            var item = myCart.SingleOrDefault(x => x.ProductId == id);
            if (item == null)
            {
                var product = _context.Products.SingleOrDefault(x => x.ProductId == id);
                item = new CartItem
                {
                    ProductId = id,
                    ProductName = product.ProductName,
                    Price = product.Price.Value,
                    Discount = product.Discount.Value,
                    Quantity = quantity,
                    Image = product.Image,
                };
                myCart.Add(item);
            }
            else
            {
                item.Quantity++;
            }
            TempData["massage"] = "The product has been successfully added to the cart!";
            //TempData.Remove("massage");
            HttpContext.Session.SetObject("cart", myCart);
            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "Products");
            }
            else
            {
                return Redirect(returnUrl);
            }
            //return RedirectToAction("Index", "Products");
        }
        public IActionResult RemoveCart(int id)
        {
            var myCart = Carts;
            var item = myCart.SingleOrDefault(x => x.ProductId == id);
            if(item != null)
            {
                myCart.Remove(item);
                HttpContext.Session.SetObject("cart", myCart);
            }
            return RedirectToAction("MyOrder", "AddToCart");
        }
        [Authorize]
        public IActionResult CheckOut()
        {
            if(Carts.Count == 0)
            {
                return RedirectToAction("Index","Products");
            }
            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return View(Carts);
        }
        [Authorize]
        [HttpPost]
        public IActionResult CheckOut(CheckOutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerId = int.Parse( HttpContext.User.Claims.SingleOrDefault(p => p.Type == "CustomerID").Value);
                var customer = new Customer();
                if (model.likeCustomer)
                {
                    customer = _context.Customers.SingleOrDefault(c => c.CustomerId == customerId);
                }
                var order = new Order
                {
                    CustomerId = customerId,
                    CustomerName = model.CustomerName ?? customer.CustomerName,
                    Address = model.Address ?? customer.Address,
                    Email = model.Email ?? customer.Email,
                    Phone = model.Phone ?? customer.Phone,
                    OrderDate = DateTime.Now,
                    TransactStatusId = 2,
                    TransportFee = 10,
                    PaymentMethod = "COD",
                };
                _context.Database.BeginTransaction();
                try
                {
                    _context.Database.CommitTransaction();
                    _context.Add(order);
                    _context.SaveChanges();

                    var orderdetail = new List<OrderDetail>();
                    foreach(var item in Carts)
                    {
                        orderdetail.Add(new OrderDetail
                        {
                            OrderId = order.OrderId,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            Discount = item.Discount,
                        });
                    }
                    _context.AddRange(orderdetail);
                    _context.SaveChanges();
                }
                catch
                {
                    _context.Database.RollbackTransaction();
                   
                }

                foreach (var item in Carts)
                {
                    var pros =  _context.Products.Where(p => p.ProductId == item.ProductId).FirstOrDefault();
                    
                    pros.Quantity = pros.Quantity - item.Quantity;
                    _context.SaveChanges();
                }

                HttpContext.Session.SetObject("cart", new List<CartItem>());

                return View("Succes");
            }
            return View(Carts);
        }
        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }
        #region Paypal payment
        [Authorize]
        [HttpPost("/AddToCart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            var totalAmount = (Carts.Sum(p => p.Total) + 10).ToString();
            var currency = "USD";
            var orderreferencenumber = "OD" + DateTime.Now.Ticks.ToString();

            try
            {
                var response = await _paypalClient.CreateOrder(totalAmount, currency, orderreferencenumber);
                return Ok(response);
            }
            catch(Exception ex)
            {
                var errors = new { ex.GetBaseException().Message };
                return BadRequest(errors);
            }
        }
        [Authorize]
        [HttpPost("/AddToCart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderId,CancellationToken cancellationToken)
        {
            var totalAmount = Carts.Sum(p => p.Total);
            try
            {
                var response = await _paypalClient.CaptureOrder(orderId);

                //if (ModelState.IsValid)
                //{

                    var customerId = int.Parse(HttpContext.User.Claims.SingleOrDefault(p => p.Type == "CustomerID").Value);
                    var customer = _context.Customers.SingleOrDefault(c => c.CustomerId == customerId);
                    

                var order = new Order
                    {
                        CustomerId = customerId,
                        CustomerName = customer.CustomerName ,
                        Address = customer.Address,
                        Email = customer.Email,
                        Phone = customer.Phone,
                        OrderDate = DateTime.Now,
                        TransactStatusId = 2,
                        TransportFee = 10,
                        PaymentMethod = "Paypal",
                        TotalAmount = totalAmount,
                    };
                    
                        try
                        {
                            _context.Add(order);
                            _context.SaveChanges();

                            var orderdetail = new List<OrderDetail>();
                            foreach (var item in Carts)
                            {
                                orderdetail.Add(new OrderDetail
                                {
                                    OrderId = order.OrderId,
                                    ProductId = item.ProductId,
                                    Quantity = item.Quantity,
                                    Price = item.Price,
                                    Discount = item.Discount,
                                });
                            }
                            _context.AddRange(orderdetail);
                            _context.SaveChanges();

                            // Giả sử Carts là danh sách các mục hàng trong đơn đặt hàng
                            foreach (var item in Carts)
                            {
                                var product = _context.Products.Find(item.ProductId);
                                if (product != null)
                                {
                                    product.Quantity -= item.Quantity;
                                    _context.SaveChanges();
                                }
                            }

                        }
                        catch
                        {
                            throw; // Ném lại lỗi để báo lỗi trở về cho người gửi yêu cầu
                        }

                    HttpContext.Session.SetObject("cart", new List<CartItem>());
                //}
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }
        #endregion

    }

}
