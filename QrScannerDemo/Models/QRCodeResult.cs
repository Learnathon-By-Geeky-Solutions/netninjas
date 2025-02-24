namespace QrScannerDemo.Models
{
    public class QRCodeResult
    {
        public string? DecodedText { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
