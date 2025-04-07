namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class PasswordResetModel
    {
        public required string FullName { get; set; }
        public required string ResetUrl { get; set; }
    }
}
