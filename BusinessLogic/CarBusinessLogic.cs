using APIDocumentationSample.Interfaces;
using APIDocumentationSample.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace APIDocumentationSample.BusinessLogic;

public class CarBusinessLogic : ICarBusinessLogic
{
    private readonly ICarDataAccess _carDataAccess;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<CarBusinessLogic> _logger;
    private const string CacheKey = "cars:list";
    private const int CacheDurationSeconds = 3600; // 1 hour

    public CarBusinessLogic(ICarDataAccess carDataAccess, IConnectionMultiplexer redis, ILogger<CarBusinessLogic> logger)
    {
        _carDataAccess = carDataAccess;
        _redis = redis;
        _logger = logger;
    }

    /// <summary>
    /// Gets car details from cache if available; otherwise, fetches from database and caches the result.
    /// </summary>
    public async Task<List<Car>> GetCarDetailsAsync()
    {
        try
        {
            // Check Redis cache
            var db = _redis.GetDatabase();
            var cachedData = await db.StringGetAsync(CacheKey);

            if (cachedData.HasValue)
            {
                _logger.LogInformation("Cache hit for cars list");
                var cars = JsonSerializer.Deserialize<List<Car>>(cachedData.ToString());
                return cars ?? new List<Car>();
            }

            _logger.LogInformation("Cache miss for cars list. Fetching from database...");

            // Cache miss - fetch from database
            var carsFromDb = await _carDataAccess.GetCarsFromStoredProcedureAsync();

            // Store in Redis with expiration
            var serialized = JsonSerializer.Serialize(carsFromDb);
            await db.StringSetAsync(CacheKey, serialized, TimeSpan.FromSeconds(CacheDurationSeconds));

            _logger.LogInformation("Stored {Count} cars in cache with {Duration}s expiration", carsFromDb.Count, CacheDurationSeconds);

            return carsFromDb;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetCarDetailsAsync");
            throw;
        }
    }
}
