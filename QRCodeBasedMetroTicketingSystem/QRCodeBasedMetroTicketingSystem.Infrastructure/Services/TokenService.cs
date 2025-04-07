using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DateTime _tokenLifetime = DateTime.UtcNow.AddHours(24);

        public TokenService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GenerateEmailVerificationToken(string email)
        {
            return await GenerateTokenAsync(email, TokenType.EmailVerification);
        }

        public async Task<string> GeneratePasswordResetToken(string email)
        {
            return await GenerateTokenAsync(email, TokenType.PasswordReset);
        }

        private async Task<string> GenerateTokenAsync(string email, TokenType tokenType)
        {
            string token = Guid.NewGuid().ToString("N");

            var userToken = new UserToken
            {
                Email = email,
                Token = token,
                Type = tokenType,
                ExpiryDate = _tokenLifetime,
            };

            await _unitOfWork.UserTokenRepository.AddTokenAsync(userToken);
            await _unitOfWork.SaveChangesAsync();

            return token;
        }

        public async Task<bool> ValidateTokenAsync(string email, TokenType tokenType, string token)
        {
            var userToken = await _unitOfWork.UserTokenRepository.GetTokenByEmailAsync(email);

            return userToken != null && userToken.Email == email && userToken.Token == token && userToken.Type == tokenType && !userToken.IsUsed && userToken.ExpiryDate > DateTime.UtcNow;
        }

        public async Task MarkTokenAsUsedAsync(string email)
        {
            var userToken = await _unitOfWork.UserTokenRepository.GetTokenByEmailAsync(email);
            if (userToken != null)
            {
                userToken.IsUsed = true;
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
