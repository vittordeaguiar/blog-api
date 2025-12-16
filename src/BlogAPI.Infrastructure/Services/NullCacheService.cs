using BlogAPI.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BlogAPI.Infrastructure.Services;

public class NullCacheService : ICacheService
{
    private readonly ILogger<NullCacheService> _logger;

    public NullCacheService(ILogger<NullCacheService> logger)
    {
        _logger = logger;
        _logger.LogWarning("Using NullCacheService - caching is disabled");
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        return Task.FromResult<T?>(null);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        return Task.CompletedTask;
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        return await factory();
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
