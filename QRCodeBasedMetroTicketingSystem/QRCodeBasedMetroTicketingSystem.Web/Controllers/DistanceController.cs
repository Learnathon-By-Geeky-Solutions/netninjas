using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Services.Implementations;
using QRCodeBasedMetroTicketingSystem.Application.Services.Interfaces;
using QRCodeBasedMetroTicketingSystem.Domain.Entities;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Services.Interfaces;
using StackExchange.Redis;

namespace QRCodeBasedMetroTicketingSystem.Web.Controllers
{
    public class DistanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistanceService _distance;
        private readonly IRedisCacheService _redis;

        public DistanceController(ApplicationDbContext context, IDistanceService distance, IRedisCacheService redis)
        {
            _context = context;
            _distance = distance;
            _redis = redis;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                List<Station> stations = await _context.Stations.ToListAsync();
                ViewBag.StationCount = stations.Count;
                return View(stations);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Index: {ex.Message}");
                ViewBag.Error = $"Error loading stations: {ex.Message}";
                return View(new List<Station>());
            }
        }
        public async Task<IActionResult> CasheDistance()
        {
            List<Station> stations = await _context.Stations.ToListAsync();
            List<StationDistance> connections = await _context.StationDistances.ToListAsync();
            int startStationId = int.MaxValue;
            foreach (var station in stations)
            {
                if (startStationId > station.StationId)
                {
                    startStationId = station.StationId;
                }
            }
            if (startStationId == int.MaxValue)
            {
                return NotFound();
            }
            var cumulativeDistance = _distance.CalculateCumulativeDistances(startStationId, stations, connections);
            _redis.CacheStationCumulativeDistances(cumulativeDistance);
            return Content("Station cumulative Distance have been cached in Redis");
        }
        public async Task<IActionResult> GetAllStations()
        {
            List<Station> stations = await _context.Stations.ToListAsync();
            return View(stations);
        }

        [HttpPost]
        public async Task<IActionResult> GetDistance(int stationId1, int stationId2)
        {
            ViewBag.MethodReached = "GetDistance method reached";

            List<Station> stations = await _context.Stations.ToListAsync();
            ViewBag.StationCount = stations.Count;

            Console.WriteLine($"Station ID 1: {stationId1}, Station ID 2: {stationId2}");

            try
            {
                var redisDb = _redis.GetDatabase();
                var distance1 = await redisDb.StringGetAsync($"station:{stationId1}:cumulative");
                var distance2 = await redisDb.StringGetAsync($"station:{stationId2}:cumulative");


                ViewBag.RedisValue1 = distance1.HasValue ? distance1.ToString() : "null";
                ViewBag.RedisValue2 = distance2.HasValue ? distance2.ToString() : "null";

                if (!distance1.HasValue || !distance2.HasValue)
                {
                    Console.WriteLine("One or both station IDs are not found in Redis.");
                    ViewBag.Error = "One or both station IDs are not found in Redis.";
                    return View("Index", stations);
                }

                if (!double.TryParse(distance1.ToString(), out double cumulativeDistance1) ||
                    !double.TryParse(distance2.ToString(), out double cumulativeDistance2))
                {
                    Console.WriteLine("Failed to parse distance values from Redis.");
                    ViewBag.Error = "Failed to parse distance values from Redis.";
                    return View("Index", stations);
                }

                double distance = Math.Abs(cumulativeDistance1 - cumulativeDistance2);

                Console.WriteLine($"Calculated Distance: {distance}");

                ViewBag.Distance = distance;
                ViewBag.StationId1 = stationId1;
                ViewBag.StationId2 = stationId2;

                return View("Index", stations);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetDistance: {ex.Message}");
                ViewBag.Error = $"Error calculating distance: {ex.Message}";
                return View("Index", stations);
            }
        }
    }
}
