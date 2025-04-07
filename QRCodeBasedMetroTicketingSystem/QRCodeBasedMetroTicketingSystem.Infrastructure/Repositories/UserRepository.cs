using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> CheckPhoneExistsAsync(string phoneNumber)
        {
            return await _dbSet.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task AddUserAsync(User user)
        {
            await _dbSet.AddAsync(user);
        }

        public async Task<User?> GetUserByPhoneAsync(string phoneNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
    }
}
