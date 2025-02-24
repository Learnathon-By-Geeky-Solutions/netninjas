using System.Diagnostics;
using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using QrScannerDemo.Models;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace QrScannerDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ScanQRCode(IFormFile qrCodeImage)
        {
            try
            {
                if (qrCodeImage == null || qrCodeImage.Length == 0)
                {
                    return Json(new QRCodeResult
                    {
                        Success = false,
                        ErrorMessage = "Please upload an image"
                    });
                }
                using var memoryStream = new MemoryStream();
                qrCodeImage.CopyTo(memoryStream);
                var bitmap = new Bitmap(memoryStream);

                var barcodeReader = new BarcodeReader
                {
                    Options = new DecodingOptions
                    {
                        PossibleFormats = new[] { BarcodeFormat.QR_CODE }
                    }
                };
                var result = barcodeReader.Decode(bitmap);

                return Json(new QRCodeResult
                {
                    Success = result != null,
                    DecodedText = result?.Text,
                    ErrorMessage = result == null ? "No QR code found" : null
                });
            }
            catch (Exception ex) 
            {
                return Json(new QRCodeResult
                {
                    Success = false,
                    ErrorMessage = "Error proccessing image: " + ex.Message
                });
            }
        }
        public IActionResult Camera()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
