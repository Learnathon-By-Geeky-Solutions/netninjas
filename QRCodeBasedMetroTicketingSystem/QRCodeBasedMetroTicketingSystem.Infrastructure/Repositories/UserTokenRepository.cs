using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Repositories
{
    public class UserTokenRepository : Repository<UserToken>, IUserTokenRepository
    {
        public UserTokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddTokenAsync(UserToken userToken)
        {
            await _dbSet.AddAsync(userToken);
        }

        public async Task<UserToken?> GetTokenAsync(string email, TokenType tokenType, string token)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.Email == email && t.Type == tokenType && t.Token == token && t.ExpiryDate > DateTime.UtcNow && !t.IsUsed);
        }
    }
}
