namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class EmailVerificationModel
    {
        public required string FullName { get; set; }
        public required string VerificationUrl { get; set; }
    }
}
