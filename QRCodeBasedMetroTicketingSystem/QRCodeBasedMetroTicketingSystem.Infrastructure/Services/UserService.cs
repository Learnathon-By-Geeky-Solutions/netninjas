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

        public async Task<bool> CheckPhoneExistsAsync(string phoneNumber)
        {
            return await _unitOfWork.UserRepository.CheckPhoneExistsAsync(BdCountryCode + phoneNumber);
        }

        public async Task<Result> RegisterUserAsync(RegisterUserDto registerDto)
        {
            try
            {
                var user = new User
                {
                    FullName = registerDto.FullName,
                    Email = registerDto.Email,
                    PhoneNumber = BdCountryCode + registerDto.PhoneNumber,
                    NID = registerDto.NID,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                };

                await _unitOfWork.UserRepository.AddUserAsync(user);
                await _unitOfWork.SaveChangesAsync();
                
                return Result.Success("Account created successfully!");
            }
            catch (Exception ex)
            {
                return Result.Failure($"An error occurred while registering the user: {ex.Message}");
            }
        }

        public async Task<(bool IsSuccess, UserDto User, string Token, string Message)> LoginUserAsync(string phoneNumber, string password)
        {
            var user = await _unitOfWork.UserRepository.GetUserByPhoneAsync(BdCountryCode + phoneNumber);
            var userDto = _mapper.Map<UserDto>(user);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return (false, userDto, string.Empty, "Invalid email or password");
            }

            if (!user.IsEmailVerified)
            {
                return (true, userDto, string.Empty, "Verify Email.");
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user.Id.ToString(), user.PhoneNumber, role);

            return (true, userDto, token, "Login successful!");
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id);
            return _mapper.Map<UserDto>(user);
        }
    }
}
