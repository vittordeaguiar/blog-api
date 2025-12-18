using BlogAPI.Application.Interfaces;
using BlogAPI.Application.Mappings;
using BlogAPI.Application.Services;
using BlogAPI.Domain.Interfaces;
using BlogAPI.Infrastructure.Data;
using BlogAPI.Infrastructure.Repositories;
using BlogAPI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace BlogAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<BlogDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);

                    npgsqlOptions.CommandTimeout(30);

                    npgsqlOptions.MigrationsAssembly(typeof(BlogDbContext).Assembly.FullName);
                });

#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
#endif
        });

        services.AddAutoMapper(typeof(PostMappingProfile).Assembly);

        AddRedisServices(services, configuration);

        services.AddSingleton<IPasswordService, BCryptPasswordService>();
        services.AddSingleton<ITokenService, TokenService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<ISlugGenerator, SlugGenerator>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }

    private static void AddRedisServices(IServiceCollection services, IConfiguration configuration)
    {
        var redisEnabled = configuration["RedisSettings:Enabled"] == "true";

        if (redisEnabled)
        {
            try
            {
                var connectionString = configuration["RedisSettings:ConnectionString"] ?? "localhost:6379";
                var password = configuration["RedisSettings:Password"];

                var redisConnection = !string.IsNullOrEmpty(password)
                    ? $"{connectionString},password={password}"
                    : connectionString;

                var configOptions = ConfigurationOptions.Parse(redisConnection);
                configOptions.ConnectTimeout = int.TryParse(configuration["RedisSettings:ConnectTimeout"], out var ct) ? ct : 5000;
                configOptions.SyncTimeout = int.TryParse(configuration["RedisSettings:SyncTimeout"], out var st) ? st : 5000;
                configOptions.ConnectRetry = int.TryParse(configuration["RedisSettings:ConnectRetry"], out var cr) ? cr : 3;
                configOptions.AbortOnConnectFail = configuration["RedisSettings:AbortOnConnectFail"] == "true";

                var redis = ConnectionMultiplexer.Connect(configOptions);

                services.AddSingleton<IConnectionMultiplexer>(redis);
                services.AddSingleton<ICacheService, RedisCacheService>();
            }
            catch (Exception ex)
            {
                var sp = services.BuildServiceProvider();
                var logger = sp.GetService<ILogger<RedisCacheService>>();
                logger?.LogWarning(ex, "Failed to connect to Redis. Using NullCacheService as fallback.");
                services.AddSingleton<ICacheService, NullCacheService>();
            }
        }
        else
        {
            services.AddSingleton<ICacheService, NullCacheService>();
        }
    }
}