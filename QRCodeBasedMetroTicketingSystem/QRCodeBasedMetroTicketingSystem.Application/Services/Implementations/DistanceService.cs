using QRCodeBasedMetroTicketingSystem.Application.Services.Interfaces;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeBasedMetroTicketingSystem.Application.Services.Implementations
{
    public class DistanceService : IDistanceService
    {
        public Dictionary<int, double> CalculateCumulativeDistances(int startStationId, List<Station> stations, List<StationDistance> connections)
        {
            Dictionary<int, double> cumulativeDistances = stations.ToDictionary(s => s.StationId, s => double.MaxValue);

            cumulativeDistances[startStationId] = 0;

            Queue<int> stationQueue = new Queue<int>();
            stationQueue.Enqueue(startStationId);

            while (stationQueue.Count > 0)
            {
                int currentStationId = stationQueue.Dequeue();
                double currentDistance = cumulativeDistances[currentStationId];

                var relatedConnections = connections.Where(c => c.FromStationId == currentStationId || c.ToStationId == currentStationId);

                foreach (var connection in relatedConnections)
                {
                    int neighborId = connection.FromStationId == currentStationId ? connection.ToStationId : connection.FromStationId;
                    double newDistance = currentDistance + (double)connection.DistanceKm;

                    if (newDistance < cumulativeDistances[neighborId])
                    {
                        cumulativeDistances[neighborId] = newDistance;
                        stationQueue.Enqueue(neighborId);
                    }
                }
            }
            const double Epsilon = 1e-9;
            Dictionary<int, double> cumlativeDistance = new Dictionary<int, double>();
            foreach (var station in stations)
            {
                double distance = cumulativeDistances[station.StationId];

                cumlativeDistance[station.StationId] =
                    Math.Abs(distance - double.MaxValue) < Epsilon ? -1 : distance;
            }
            return cumlativeDistance;
        }
    }
}
