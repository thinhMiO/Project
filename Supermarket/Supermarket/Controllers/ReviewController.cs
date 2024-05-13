using Microsoft.AspNetCore.Mvc;
using Supermarket.DataModels;
using Supermarket.Models;
using System.Data.Entity;

namespace Supermarket.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ShopContext _context;

        public ReviewController(ShopContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRating(ReviewVM model)
        {

            var customerId = int.Parse(HttpContext.User.Claims.SingleOrDefault(p => p.Type == "CustomerID").Value);
            
            if (ModelState.IsValid)
            {
                if(model.Contents == null)
                {
                    ModelState.AddModelError("Error", "Content cannot be empty.");
                }
                else
                {
                    var review = new Review
                    {
                        CustomerId = customerId,
                        ProductId = model.ProductId,
                        Contents = model.Contents,
                        Status = true,
                        CreateDate = DateTime.Now,

                    };
                    _context.Reviews.Add(review);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("details", "Products", new { id = model.ProductId });
            }
            // Xử lý lỗi nếu cần
            return RedirectToAction("details", "Products", new { id = model.ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditRating(int ReviewId, string Contents)
        {
            try
            {
                // Tìm đánh giá cần chỉnh sửa trong cơ sở dữ liệu
                var reviewToUpdate = _context.Reviews.Where(r =>r.ReviewId == ReviewId).FirstOrDefault();

                if (reviewToUpdate == null)
                {
                    return NotFound(); // Trả về NotFound nếu không tìm thấy đánh giá
                }

                // Cập nhật nội dung của đánh giá
                reviewToUpdate.Contents = Contents;

                // Lưu các thay đổi vào cơ sở dữ liệu
                _context.Update(reviewToUpdate);
                _context.SaveChanges();

                //return Json(new { success = true });
                return RedirectToAction("details", "Products", new { id = reviewToUpdate.ProductId });
            }
            catch (Exception ex)
            {
                var errors = new { ex.GetBaseException().Message };
                return BadRequest(errors);
                //return Json(new { success = false }); // Trả về kết quả thất bại dưới dạng JSON
            }
            
        }


    }
}
