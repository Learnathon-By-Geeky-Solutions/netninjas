using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<bool> CheckEmailExistsAsync(string email);
        Task<bool> CheckPhoneExistsAsync(string phone);
        Task AddUserAsync(User user);
        Task<User?> GetUserByPhoneAsync(string phone);
        Task<User?> GetUserByIdAsync(int id);
    }
}
