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

        services.AddSingleton<IPasswordService, BCryptPasswordService>();
        services.AddSingleton<ITokenService, TokenService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}