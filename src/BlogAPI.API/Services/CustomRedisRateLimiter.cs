using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace BlogAPI.API.Services;

public class CustomRedisRateLimiter : IRateLimiterPolicy<string>
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<CustomRedisRateLimiter> _logger;
    private readonly int _permitLimit;
    private readonly TimeSpan _window;

    public CustomRedisRateLimiter(
        IConnectionMultiplexer redis,
        ILogger<CustomRedisRateLimiter> logger,
        int permitLimit,
        TimeSpan window)
    {
        _redis = redis;
        _logger = logger;
        _permitLimit = permitLimit;
        _window = window;
    }

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; } = (context, cancellationToken) =>
    {
        if (context.Lease.TryGetMetadata("RetryAfter", out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = ((TimeSpan)retryAfter!).TotalSeconds.ToString(CultureInfo.InvariantCulture);
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        return ValueTask.CompletedTask;
    };

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        var identifier = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"rate_limit:{identifier}";

        return RateLimitPartition.Get(key, _ => new RedisRateLimiter(
            _redis,
            _logger,
            key,
            _permitLimit,
            _window));
    }
}
