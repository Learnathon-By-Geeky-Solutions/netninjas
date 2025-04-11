namespace QRCodeBasedMetroTicketingSystem.Application.DTOs
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string ExpiryInMinutes { get; set; } = null!;
    }
}
