using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^01[3-9]\d{8}$", ErrorMessage = "Please enter a valid Bangladeshi mobile number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
