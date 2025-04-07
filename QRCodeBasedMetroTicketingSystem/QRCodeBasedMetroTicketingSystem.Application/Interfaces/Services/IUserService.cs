using QRCodeBasedMetroTicketingSystem.Application.Common.Result;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<bool> CheckEmailExistsAsync(string email);
        Task<bool> CheckPhoneExistsAsync(string phone);
        Task<Result> RegisterUserAsync(RegisterUserDto registerDto);
        Task<(bool IsSuccess, UserDto User, string Token, string Message)> LoginUserAsync(string phoneNumber, string password);
        Task<UserDto?> GetUserByIdAsync(int UserId);
    }
}
