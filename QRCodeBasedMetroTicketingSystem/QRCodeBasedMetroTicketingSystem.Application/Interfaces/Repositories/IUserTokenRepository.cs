using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface IUserTokenRepository : IRepository<UserToken>
    {
        Task AddTokenAsync(UserToken userToken);
        Task<UserToken?> GetTokenAsync(string email, TokenType tokenType, string token);
    }
}
