﻿using QRCodeBasedMetroTicketingSystem.Domain.Entities;

namespace QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services
{
    public interface IQRCodeService
    {
        string GenerateQRCodeData(Ticket ticket);
        string GenerateQRCode(string qrCodeData);
        bool ValidateQRCodeData(string qrCodeData);
        int ParseQRCodeDataToGetTicketId(string qrCodeData);
    }
}
