namespace BlogAPI.API.Configuration;

public class RedisSettings
{
    public bool Enabled { get; init; }
    public string ConnectionString { get; init; } = string.Empty;
    public int DefaultExpirationMinutes { get; init; } = 10;
    public int RetryCount { get; init; } = 3;
    public int ConnectTimeout { get; init; } = 5000;
    public int SyncTimeout { get; init; } = 5000;
}