using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services.Interfaces
{
    public interface IRedisCacheService
    {
        void CacheStationCumulativeDistances(Dictionary<int, double> cumlativeDistance);
        IDatabase GetDatabase();
    }
}
