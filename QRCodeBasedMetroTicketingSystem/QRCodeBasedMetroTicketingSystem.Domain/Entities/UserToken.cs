namespace QRCodeBasedMetroTicketingSystem.Domain.Entities
{
    public class UserToken
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string Token { get; set; }
        public TokenType Type { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; } = false;
    }

    public enum TokenType
    {
        EmailVerification = 1,
        PasswordReset = 2
    }
}
