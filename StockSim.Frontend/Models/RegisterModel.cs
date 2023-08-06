using System.ComponentModel.DataAnnotations;

namespace StockSim.Frontend.Models {
    public class RegisterModel {
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(128, ErrorMessage = "Email address must be at most 128 characters in length")]
        [EmailAddress(ErrorMessage = "Email address is improperly formatted")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [MaxLength(32, ErrorMessage = "Username must be at most 32 characters in length")]
        [RegularExpression(@"[^%\\\?""\'~/\$\*\{\}\s]+", ErrorMessage = "Username contains invalid characters")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters in length")]
        [MaxLength(256, ErrorMessage = "Password must be at most 256 characters in length")]
        [RegularExpression(@"[^%\\\?""\'~/\$\*\{\}\s]+", ErrorMessage = "Password contains invalid characters")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
    }
}
