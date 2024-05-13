using System.ComponentModel.DataAnnotations;

namespace Supermarket.DataModels
{
    public class ReviewVM
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Please add content")]
        public string? Contents { get; set; }
    }
}
