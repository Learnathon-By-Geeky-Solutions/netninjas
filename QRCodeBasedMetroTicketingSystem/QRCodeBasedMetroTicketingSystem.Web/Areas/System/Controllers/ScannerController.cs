using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Web.Areas.System.ViewModels;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.System.Controllers
{
    [Area("System")]
    public class ScannerController : Controller
    {
        // There will be used service to handle ticket validation logic

        // Main QR Code Scanner View
        public IActionResult Index()
        {
            return View();
        }

        // Endpoint for scanning QR code
        [HttpPost]
        [Route("ticket/scan")]
        public async Task<IActionResult> ScanTicket([FromBody] ScanTicketRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            /// <summary>
            /// Validates the QR code and returns the ticket status.
            /// This will later be connected to a database and include business logic.
            /// </summary>
            /// <param name="qrCode">The scanned QR code string from the user/device.</param>
            /// <returns>Ticket validation result (Valid/Invalid with message and metadata).</returns>
            /// 
            if (request == null || string.IsNullOrEmpty(request.Token))
            {
                return Json(new
                {
                    IsValid = false,
                    Message = "Invalid QR code data"
                });
            }
            else if (request.Token[0] >= 'a' && request.Token[0] <= 'z')
            {
                return Json(new
                {
                    IsValid = true,
                    Message = "Valid QR code data"
                });
            }

            return Json(new
            {
                IsValid = false,
                Message = "Invalid QR code data"
            });
        }
    }
}
