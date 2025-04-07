using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [RegularExpression(@"^[A-Za-z\s.]+$", ErrorMessage = "Full Name must be non-empty and contain only letters, spaces, and periods")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^01[3-9]\d{8}$", ErrorMessage = "Phone number must be a valid Bangladesh mobile number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "NID number is required")]
        [RegularExpression(@"^\d{10}$|^\d{17}$", ErrorMessage = "NID must be either 10 or 17 digits long and contain only numbers")]
        public string NID { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "Password must be at least 8 characters with at least one letter and one number")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "You must agree to the terms and conditions")]
        public bool TermsAgreed { get; set; }
    }
}
