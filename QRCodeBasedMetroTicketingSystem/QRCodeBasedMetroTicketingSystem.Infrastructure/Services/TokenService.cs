using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GenerateEmailVerificationToken(string email)
        {
            DateTime tokenLifetime = DateTime.UtcNow.AddHours(24);
            return await GenerateTokenAsync(email, TokenType.EmailVerification, tokenLifetime);
        }

        public async Task<string> GeneratePasswordResetToken(string email)
        {
            DateTime tokenLifetime = DateTime.UtcNow.AddMinutes(30);
            return await GenerateTokenAsync(email, TokenType.PasswordReset, tokenLifetime);
        }

        private async Task<string> GenerateTokenAsync(string email, TokenType tokenType, DateTime tokenLifetime)
        {
            string token = Guid.NewGuid().ToString("N");

            var userToken = new UserToken
            {
                Email = email,
                Token = token,
                Type = tokenType,
                ExpiryDate = tokenLifetime,
            };

            await _unitOfWork.UserTokenRepository.AddTokenAsync(userToken);
            await _unitOfWork.SaveChangesAsync();

            return token;
        }

        public async Task<UserToken?> GetValidTokenAsync(string email, TokenType tokenType, string token)
        {
            return await _unitOfWork.UserTokenRepository.GetTokenAsync(email, tokenType, token);
        }
    }
}
