using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface ITokenService
    {
        Task<string> GenerateEmailVerificationToken(string email);
        Task<string> GeneratePasswordResetToken(string email);
        Task<UserToken?> GetValidTokenAsync(string email, TokenType tokenType, string token);
    }
}
