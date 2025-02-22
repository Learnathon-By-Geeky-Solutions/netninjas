using QRCodeBasedMetroTicketingSystem.Services.Interface;
using System.Security.Claims;

namespace QRCodeBasedMetroTicketingSystem.Services.Implementation
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserInfoService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
