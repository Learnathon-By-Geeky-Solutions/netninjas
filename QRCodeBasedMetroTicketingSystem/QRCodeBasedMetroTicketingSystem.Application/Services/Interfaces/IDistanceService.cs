using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeBasedMetroTicketingSystem.Application.Services.Interfaces
{
    public interface IDistanceService
    {
        Dictionary<int, double> CalculateCumulativeDistances(int startStationId, List<Station> stations, List<StationDistance> connections);
    }
}
