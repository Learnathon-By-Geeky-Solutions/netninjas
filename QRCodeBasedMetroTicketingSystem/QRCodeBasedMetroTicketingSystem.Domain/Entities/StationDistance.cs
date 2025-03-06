using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeBasedMetroTicketingSystem.Domain.Entities
{
    public class StationDistance
    {
        public int Id { get; set; }
        public int FromStationId { get; set; }
        public int ToStationId { get; set; }
        public decimal DistanceKm { get; set; }
    }
}
