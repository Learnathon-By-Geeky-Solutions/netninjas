using System.ComponentModel.DataAnnotations;

namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}
