using System.Text.Json;
using Ardalis.Result;
using Microsoft.Extensions.Caching.Distributed;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class CacheRepository
{
    private readonly IDistributedCache _cache;

    public CacheRepository(IDistributedCache cache) => _cache = cache;

    public async Task<Result<T>> Get<T>(string cacheKey)
    {
        try
        {
            var cachedJson = await _cache.GetStringAsync(cacheKey);

            if (cachedJson is null)
                return Result<T>.NotFound();

            return Result<T>.Success(JsonSerializer.Deserialize<T>(cachedJson)!);
        }
        catch (Exception exception)
        {
            return Result<T>.Error(exception.Message);
        }
    }

    public async Task<Result> Set<T>(string cacheKey, T value, TimeSpan timeToLive)
    {
        try
        {
            var entryOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeToLive };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(value), entryOptions);
            
            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Error(exception.Message);
        }
    }

    public async Task<Result> Remove(string cacheKey)
    {
        try
        {
            await _cache.RemoveAsync(cacheKey);

            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Error(exception.Message);
        }
    }
}
