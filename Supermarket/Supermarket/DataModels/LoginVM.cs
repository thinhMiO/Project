using System.ComponentModel.DataAnnotations;

namespace Supermarket.DataModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Please enter a email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; }

        [StringLength(maximumLength: 50, MinimumLength = 10, ErrorMessage = "Length must be between 10 to 50")]
        [DataType(DataType.Password)]
        public string Password { get; set; }    
    }
}
