﻿namespace QRCodeBasedMetroTicketingSystem.Application.Common.Models.DataTables
{
    public class DataTablesResponse<T>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<T> Data { get; set; }
        public string? Error { get; set; }
    }
}
