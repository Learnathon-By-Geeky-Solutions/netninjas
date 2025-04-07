using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface IUserTokenRepository : IRepository<UserToken>
    {
        Task AddTokenAsync(UserToken userToken);
        Task<UserToken?> GetTokenByEmailAsync(string email);
    }
}
