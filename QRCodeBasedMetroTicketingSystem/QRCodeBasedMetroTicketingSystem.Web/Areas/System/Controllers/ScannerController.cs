using Microsoft.AspNetCore.Mvc;
using QRCodeBasedMetroTicketingSystem.Web.Areas.System.ViewModels;

namespace QRCodeBasedMetroTicketingSystem.Web.Areas.System.Controllers
{
    [Area("System")]
    [Route("System/[controller]")]
    public class ScannerController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        // Endpoint for scanning QR code
        [HttpPost]
        [Route("ticket/scan")]
        public async Task<IActionResult> ScanTicket([FromBody] ScanTicketRequest? request)
        {
            if (request == null)
            {
                return BadRequest(new { IsValid = false, Message = "Request is null" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(request.Token))
            {
                return Json(new
                {
                    IsValid = false,
                    Message = "Invalid QR code data"
                });
            }

            if (char.IsLower(request.Token[0]))
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
