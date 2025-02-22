using QRCodeBasedMetroTicketingSystem.Models;
using QRCodeBasedMetroTicketingSystem.Services.Interface;
using QRCoder;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QRCodeBasedMetroTicketingSystem.Services.Implementation
{
    public class QRCodeService : IQRCodeService
    {
        private readonly IUserInfoService _userInfoService;

        public QRCodeService(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }
        
        public byte[] GenerateQRCode()
        {
            string userId = _userInfoService.GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("User details not found. Please log in.");
            }

            var checkInData = new StationCheckInData
            {
                UserId = userId,
                QrCodeId = Guid.NewGuid().ToString(),
                GenrationTime = DateTime.UtcNow,
                ExpirationTime = DateTime.UtcNow.AddMinutes(30)
            };

            string qrData = checkInData.ToQRString();
            Console.WriteLine($"Generated QR Data {qrData}");

            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new PngByteQRCode(qrCodeData))
                {
                    return qrCode.GetGraphic(20);
                }
            }
        }
    }
}
