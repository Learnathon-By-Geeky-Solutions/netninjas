namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class PasswordResetConfirmationEmailModel
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string DateTime { get; set; }
        public string Device { get; set; } = "Unknown Device";
        public string Location { get; set; } = "Dhaka, Bangladesh";
    }
}
