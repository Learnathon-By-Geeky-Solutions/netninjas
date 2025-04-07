namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailVerificationAsync(string email, string fullName, string verificationUrl);
        Task SendPasswordResetEmailAsync(string email, string fullName, string resetUrl);
    }
}
