using System.Threading.RateLimiting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace BlogAPI.API.Services;

public class RedisRateLimiter(
    IConnectionMultiplexer redis,
    ILogger logger,
    string key,
    int permitLimit,
    TimeSpan window)
    : RateLimiter
{
    public override TimeSpan? IdleDuration => null;

    public override RateLimiterStatistics? GetStatistics() => null;

    protected override RateLimitLease AttemptAcquireCore(int permitCount)
        => AttemptAcquireCoreAsync(permitCount).GetAwaiter().GetResult();

    protected override async ValueTask<RateLimitLease> AcquireAsyncCore(int permitCount, CancellationToken cancellationToken = default)
        => await AttemptAcquireCoreAsync(permitCount, cancellationToken);

    private async Task<RateLimitLease> AttemptAcquireCoreAsync(int permitCount, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = redis.GetDatabase();
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var windowStart = now - (long)window.TotalMilliseconds;

            var transaction = db.CreateTransaction();

            var removeOldTask = transaction.SortedSetRemoveRangeByScoreAsync(key, 0, windowStart);
            var addCurrentTask = transaction.SortedSetAddAsync(key, now, now);
            var expireTask = transaction.KeyExpireAsync(key, window);
            var countTask = transaction.SortedSetLengthAsync(key);

            await transaction.ExecuteAsync();

            await removeOldTask;
            await addCurrentTask;
            await expireTask;
            var currentCount = await countTask;

            if (currentCount <= permitLimit)
            {
                return new RedisRateLimitLease(true, null);
            }

            var retryAfter = TimeSpan.FromMilliseconds(window.TotalMilliseconds);
            return new RedisRateLimitLease(false, retryAfter);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Redis rate limiter error for key: {Key}. Failing open.", key);
            return new RedisRateLimitLease(true, null);
        }
    }

    protected override void Dispose(bool disposing) => base.Dispose(disposing);

    protected override ValueTask DisposeAsyncCore() => ValueTask.CompletedTask;

    private class RedisRateLimitLease(bool isAcquired, TimeSpan? retryAfter) : RateLimitLease
    {
        public override bool IsAcquired => isAcquired;

        public override IEnumerable<string> MetadataNames
        {
            get
            {
                if (retryAfter.HasValue)
                {
                    yield return "RetryAfter";
                }
            }
        }

        public override bool TryGetMetadata(string metadataName, out object? metadata)
        {
            if (metadataName == "RetryAfter" && retryAfter.HasValue)
            {
                metadata = retryAfter.Value;
                return true;
            }

            metadata = null;
            return false;
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}