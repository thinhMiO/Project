using System.ComponentModel.DataAnnotations;

namespace Supermarket.DataModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage ="*")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters")]
        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "*")]
        [MaxLength(150, ErrorMessage = "Maximum 150 characters")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Please enter a email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "*")]
        [MaxLength(10,ErrorMessage = "Maximum 10 characters")]
        [RegularExpression(@"0[1-9][0-9]{8}", ErrorMessage = "Not in correct format '0975144678'")]
        public string? Phone { get; set; }

        [StringLength(maximumLength: 50, MinimumLength = 10, ErrorMessage = "Length must be between 10 to 50")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool Gender { get; set; } = true;
    }
}
