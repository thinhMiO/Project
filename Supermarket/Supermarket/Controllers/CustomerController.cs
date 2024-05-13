using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Supermarket.DataModels;
using Supermarket.Extension;
using Supermarket.Models;
using Supermarket.Util;
using Microsoft.EntityFrameworkCore;

namespace Supermarket.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ShopContext db;
        private readonly IMapper _mapper;
        public CustomerController(ShopContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }
        public bool IsEmailTaken(string email)
        {
            return db.Customers.Any(a => a.Email == email);
        }
        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterVM model)
        {

            if (IsEmailTaken(model.Email) == true)
            {
                ModelState.AddModelError("Email", "Email is already taken.");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var customer = _mapper.Map<Customer>(model);
                        customer.RandomKey = MyRamdomkey.GenerateRamdoKey();
                        customer.Password = model.Password.ToMd5Hash(customer.RandomKey);
                        customer.Active = true;
                        customer.CareateDate = DateTime.UtcNow;
                        customer.RoleId = 2;

                        db.Add(customer);
                        db.SaveChanges();
                        return RedirectToAction("Login", "Customer");
                    }
                    catch
                    {

                    }
                }
            }
           
           
            return View();
        }
        #endregion
        #region Login
        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var customer = db.Customers.Include(o => o.Role).SingleOrDefault(c => c.Email == model.Email);
                
                if (customer == null)
                {
                    ModelState.AddModelError("Error", "These credentials do not match our records.");
                }
                else
                {
                    if ((bool)!customer.Active)
                    {
                        ModelState.AddModelError("Error", "You have been banned from this website. Please contact the Board Administrator for more information.");
                    }
                    else
                    {
                        if (customer.Password != model.Password.ToMd5Hash(customer.RandomKey))
                        {
                            ModelState.AddModelError("Error", "These credentials do not match our records.");
                        }
                        else
                        {
                            string cus = customer.CustomerId.ToString();
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, customer.Email),
                                new Claim(ClaimTypes.Name, customer.CustomerName),
                                new Claim(ClaimTypes.Role,customer.Role.RoleName),
                                new Claim("CustomerID", cus),

                            };
                            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimPrincipal = new ClaimsPrincipal(claimIdentity);

                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,claimPrincipal);
                            customer.LastLogin = DateTime.Now;
                            db.SaveChanges();

                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                var c = customer.RoleId;
                                //return RedirectToAction("Index", "Admin");// Redirect("/");
                                return c == 1 ? RedirectToAction("Index", "Admin") : RedirectToAction("Index", "Home");
                            }
                        }
                    }
                }
            }
            return View();
        }
        #endregion

        [Authorize]
        public IActionResult Profile()
        {
            var customerIdClaim = HttpContext.User.FindFirst("CustomerID");
            if (customerIdClaim != null)
            {
                int customerId = int.Parse(customerIdClaim.Value);
                var customer = db.Customers.FirstOrDefault(c => c.CustomerId == customerId);
                if (customer != null)
                {
                    return View(customer);
                }
            }
            return NotFound();
        }
        [Authorize]
        public IActionResult OrderHistory()
        {
            return View();

        }
        [Authorize]
        [HttpGet]
        public IActionResult ChangeProfile()
        {
            var customerIdClaim = HttpContext.User.FindFirst("CustomerID");
            if (customerIdClaim != null)
            {
                int customerId = int.Parse(customerIdClaim.Value);
                var customer = db.Customers.FirstOrDefault(c => c.CustomerId == customerId);
                if (customer != null)
                {
                    return View(customer);
                }
            }
            return NotFound();
        }
        [Authorize]
        [HttpPost]
        public IActionResult ChangeProfile(Customer updatedCustomer, IFormFile image)
        {
            if (ModelState.IsValid == false)
            {
                var customerIdClaim = HttpContext.User.FindFirst("CustomerID");
                if (customerIdClaim != null)
                {
                    int customerId = int.Parse(customerIdClaim.Value);
                    var existingCustomer = db.Customers.FirstOrDefault(c => c.CustomerId == customerId);
                    if (existingCustomer != null)
                    {
                        existingCustomer.CustomerName = updatedCustomer.CustomerName;
                        existingCustomer.Address = updatedCustomer.Address;
                        existingCustomer.Phone = updatedCustomer.Phone;
                        existingCustomer.Email = updatedCustomer.Email;
                        if (image != null && image.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                image.CopyTo(ms);
                                existingCustomer.Image = ms.ToArray();
                            }
                        }

                        db.SaveChanges();
                        return RedirectToAction("Profile");
                    }
                }
                return NotFound();
            }
            return View("Profile", updatedCustomer); 
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                var customer = db.Customers.SingleOrDefault(c => c.Email == userEmail);

                if (customer == null)
                {
                    return RedirectToAction("Login");
                }

                if (customer.Password != model.OldPassword.ToMd5Hash(customer.RandomKey))
                {
                    ModelState.AddModelError("OldPassword", "Old password is incorrect.");
                    return View(model);
                }

                customer.Password = model.NewPassword.ToMd5Hash(customer.RandomKey);
                db.Update(customer);
                await db.SaveChangesAsync();

                return RedirectToAction("Profile", "Customer");
            }

            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Customer");
        }
    }
}
