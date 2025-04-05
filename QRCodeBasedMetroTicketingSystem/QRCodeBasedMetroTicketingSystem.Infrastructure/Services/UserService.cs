using AutoMapper;
using QRCodeBasedMetroTicketingSystem.Application.Common.Result;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Repositories;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private const string BdCountryCode = "+88";
        private const string role = "User";

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtService = jwtService;
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _unitOfWork.UserRepository.CheckEmailExistsAsync(email);
        }

        public async Task<bool> CheckPhoneExistsAsync(string phone)
        {
            return await _unitOfWork.UserRepository.CheckPhoneExistsAsync(BdCountryCode + phone);
        }

        public async Task<Result> RegisterUserAsync(RegisterUserDto registerDto)
        {
            try
            {
                var user = new User
                {
                    FullName = registerDto.FullName,
                    Email = registerDto.Email,
                    Phone = BdCountryCode + registerDto.Phone,
                    NID = registerDto.NID,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                };

                await _unitOfWork.UserRepository.AddUserAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Account created successfully! Please login.");
            }
            catch (Exception ex)
            {
                return Result.Failure($"An error occurred while registering the user: {ex.Message}");
            }
        }

        public async Task<(bool IsSuccess, int UserId, string FullName, string Token, string Message)> LoginUserAsync(string phoneNumber, string password)
        {
            var user = await _unitOfWork.UserRepository.GetUserByPhoneAsync(BdCountryCode + phoneNumber);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return (false, 0, string.Empty, string.Empty, "Invalid email or password");
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Phone, role);

            return (true, user.Id, user.FullName, token, "Login successful!");
        }
    }
}
