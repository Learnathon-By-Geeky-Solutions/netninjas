using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string email, string subject, string message);
        Task<bool> SendEmailVerificationAsync(string email, string fullName, string verificationUrl);
        Task<bool> SendPasswordResetConfirmationMail(PasswordResetConfirmationEmailModel model);
        Task<bool> SendPasswordResetEmailAsync(string email, string fullName, string resetUrl);
    }
}
