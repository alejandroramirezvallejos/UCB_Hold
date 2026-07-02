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
        var cachedJson = await _cache.GetStringAsync(cacheKey);

        if (cachedJson is null)
            return Result<T>.NotFound();

        var cachedValue = JsonSerializer.Deserialize<T>(cachedJson);

        return cachedValue is null ? Result<T>.NotFound() : Result<T>.Success(cachedValue);
    }

    public async Task<Result> Set<T>(string cacheKey, T value, TimeSpan timeToLive)
    {
        var entryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = timeToLive,
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(value), entryOptions);

        return Result.Success();
    }

    public async Task<Result> Remove(string cacheKey)
    {
        await _cache.RemoveAsync(cacheKey);

        return Result.Success();
    }
}
