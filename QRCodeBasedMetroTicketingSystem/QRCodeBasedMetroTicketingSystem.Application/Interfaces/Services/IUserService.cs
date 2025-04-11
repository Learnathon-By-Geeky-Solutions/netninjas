using QRCodeBasedMetroTicketingSystem.Application.Common.Result;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<bool> CheckEmailExistsAsync(string email);
        Task<bool> CheckPhoneExistsAsync(string phone);
        Task<UserDto?> GetUserByIdAsync(int UserId);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<Result> RegisterUserAsync(RegisterUserDto registerDto);
        Task<(bool IsSuccess, UserDto User, string Token, string Message)> LoginUserAsync(string phoneNumber, string password);
        
        Task<Result> VerifyEmailAsync(string email, string token);
        Task<Result> ResetPassword(ResetPasswordModel model);
    }
}
