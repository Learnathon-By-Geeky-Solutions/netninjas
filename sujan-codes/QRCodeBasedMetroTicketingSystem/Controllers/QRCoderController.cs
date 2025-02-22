using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Services.Interface;

namespace QRCodeBasedMetroTicketingSystem.Controllers
{
    public class QRCoderController : Controller
    {
        private readonly IQRCodeService _qrCodeService;

        public QRCoderController(IQRCodeService qrCodeService)
        {
            _qrCodeService = qrCodeService;
        }

        public IActionResult Generate() => View();
        [HttpPost]
        public IActionResult GenerateQRCode()
        {
            try
            {
                byte[] qrCodeImage = _qrCodeService.GenerateQRCode();
                string qrCodeBase64 = Convert.ToBase64String(qrCodeImage);
                return View("Display", qrCodeBase64);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error generating QR code: {ex.Message}");
                return View("Generated");
            }
            
        }
        public IActionResult Display(string qrCodeBase64) => View("Display", qrCodeBase64);
    }
}
