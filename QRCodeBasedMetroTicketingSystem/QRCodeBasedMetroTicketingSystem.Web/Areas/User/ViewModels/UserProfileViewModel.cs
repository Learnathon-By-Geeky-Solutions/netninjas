﻿namespace QRCodeBasedMetroTicketingSystem.Web.Areas.User.ViewModels
{
    public class UserProfileViewModel
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? NID { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
