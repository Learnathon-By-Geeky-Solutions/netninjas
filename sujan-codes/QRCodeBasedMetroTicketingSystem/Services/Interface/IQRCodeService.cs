using QRCodeBasedMetroTicketingSystem.Models;

namespace QRCodeBasedMetroTicketingSystem.Services.Interface
{
    public interface IQRCodeService
    {
        byte[] GenerateQRCode();
    }
}
