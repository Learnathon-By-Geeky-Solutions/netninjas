using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Services.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services.Implementations
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }
        public void CacheStationCumulativeDistances(Dictionary<int, double> cumulativeDistances)
        {
            IDatabase redisDb = _redis.GetDatabase();

            foreach (var kvp in cumulativeDistances)
            {
                int stationId = kvp.Key;
                double cumulativeDistance = kvp.Value;

                string key = $"station:{stationId}:cumulative";
                redisDb.StringSet(key, cumulativeDistance);
            }
        }
        public IDatabase GetDatabase()
        {
            return _redis.GetDatabase();
        }
    }
}
