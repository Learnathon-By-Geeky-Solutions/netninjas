namespace QRCodeBasedMetroTicketingSystem.Models
{
    public class StationCheckInData
    {
        public string UserId { get; set; }
        public string QrCodeId { get; set; }
        public DateTime GenrationTime { get; set; }
        public DateTime ExpirationTime { get; set; }

        public string ToQRString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
