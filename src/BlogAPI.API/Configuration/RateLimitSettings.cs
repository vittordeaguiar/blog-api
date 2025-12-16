namespace BlogAPI.API.Configuration;

public class RateLimitSettings
{
    public const string SectionName = "RateLimitSettings";

    public bool Enabled { get; set; } = true;
    public GlobalLimitPolicy Global { get; set; } = new();
    public IpLimitPolicy IpLimit { get; set; } = new();
    public UserLimitPolicy UserLimit { get; set; } = new();
    public int StatusCode { get; set; } = StatusCodes.Status429TooManyRequests;
    public bool QueueRequests { get; set; } = false;

    public class GlobalLimitPolicy
    {
        public bool Enabled { get; set; } = true;
        public int PermitLimit { get; set; } = 1000;
        public int WindowInSeconds { get; set; } = 60;
    }

    public class IpLimitPolicy
    {
        public bool Enabled { get; set; } = true;
        public int PermitLimit { get; set; } = 100;
        public int WindowInSeconds { get; set; } = 60;
    }

    public class UserLimitPolicy
    {
        public bool Enabled { get; set; } = true;
        public int PermitLimit { get; set; } = 200;
        public int WindowInSeconds { get; set; } = 60;
    }
}
