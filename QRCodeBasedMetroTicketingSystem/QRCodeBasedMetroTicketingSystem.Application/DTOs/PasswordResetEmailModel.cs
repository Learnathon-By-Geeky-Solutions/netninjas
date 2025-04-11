namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class PasswordResetEmailModel
    {
        public required string FullName { get; set; }
        public required string ResetUrl { get; set; }
    }
}
