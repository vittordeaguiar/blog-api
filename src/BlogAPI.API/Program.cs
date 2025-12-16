using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using BlogAPI.API.Configuration;
using BlogAPI.Infrastructure;
using BlogAPI.Infrastructure.Data;
using BlogAPI.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    var corsSettings = builder.Configuration
        .GetSection(CorsSettings.SectionName)
        .Get<CorsSettings>();

    if (corsSettings != null && corsSettings.AllowedOrigins.Length > 0)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(corsSettings.PolicyName, policy =>
            {
                policy.WithOrigins(corsSettings.AllowedOrigins)
                      .WithMethods(corsSettings.AllowedMethods)
                      .WithHeaders(corsSettings.AllowedHeaders)
                      .SetPreflightMaxAge(TimeSpan.FromSeconds(corsSettings.MaxAge));

                if (corsSettings.AllowCredentials)
                {
                    policy.AllowCredentials();
                }
            });
        });
    }
}

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Servers = [new OpenApiServer { Url = "http://localhost:8080" }];

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Insira seu token JWT no formato: Bearer {seu_token}"
        });

        document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        return Task.CompletedTask;
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Em prod deve ser true
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

var rateLimitSettings = builder.Configuration
    .GetSection(RateLimitSettings.SectionName)
    .Get<RateLimitSettings>() ?? new RateLimitSettings();

var redisSettings = builder.Configuration
    .GetSection("RedisSettings")
    .Get<RedisSettings>();

if (rateLimitSettings.Enabled)
{
    var useRedisRateLimiting = redisSettings?.Enabled == true;

    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = rateLimitSettings.StatusCode;

        if (rateLimitSettings.Global.Enabled && !useRedisRateLimiting)
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: "global",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimitSettings.Global.PermitLimit,
                        Window = TimeSpan.FromSeconds(rateLimitSettings.Global.WindowInSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = rateLimitSettings.QueueRequests ? 1 : 0
                    });
            });
        }

        if (useRedisRateLimiting)
        {
            var serviceProvider = builder.Services.BuildServiceProvider();
            var redis = serviceProvider.GetService<IConnectionMultiplexer>();
            var logger = serviceProvider.GetRequiredService<ILogger<CustomRedisRateLimiter>>();

            if (redis != null)
            {
                var redisRateLimiter = new CustomRedisRateLimiter(
                    redis,
                    logger,
                    rateLimitSettings.IpLimit.PermitLimit,
                    TimeSpan.FromSeconds(rateLimitSettings.IpLimit.WindowInSeconds));

                options.AddPolicy("api-limit", redisRateLimiter);
            }
            else
            {
                AddInMemoryRateLimitPolicy(options, rateLimitSettings);
            }
        }
        else
        {
            AddInMemoryRateLimitPolicy(options, rateLimitSettings);
        }

        options.OnRejected = async (context, cancellationToken) =>
        {
            context.HttpContext.Response.StatusCode = rateLimitSettings.StatusCode;

            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                context.HttpContext.Response.Headers.RetryAfter =
                    retryAfter.TotalSeconds.ToString();
            }

            var problemDetails = new
            {
                type = "https://tools.ietf.org/html/rfc6585#section-4",
                title = "Too Many Requests",
                status = rateLimitSettings.StatusCode,
                detail = "Rate limit exceeded. Please try again later.",
                instance = context.HttpContext.Request.Path.Value
            };

            await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        };
    });
}

static void AddInMemoryRateLimitPolicy(RateLimiterOptions options, RateLimitSettings rateLimitSettings)
{
    options.AddPolicy("api-limit", context =>
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? "anonymous";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: $"user-{userId}",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = rateLimitSettings.UserLimit.PermitLimit,
                    Window = TimeSpan.FromSeconds(rateLimitSettings.UserLimit.WindowInSeconds),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = rateLimitSettings.QueueRequests ? 1 : 0
                });
        }
        else
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString()
                ?? context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? "unknown";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: $"ip-{ipAddress}",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = rateLimitSettings.IpLimit.PermitLimit,
                    Window = TimeSpan.FromSeconds(rateLimitSettings.IpLimit.WindowInSeconds),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = rateLimitSettings.QueueRequests ? 1 : 0
                });
        }
    });
}

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Auto-apply migrations on startup (Development only)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<BlogDbContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Applying database migrations...");
            context.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }
    }
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    var corsSettings = app.Configuration
        .GetSection(CorsSettings.SectionName)
        .Get<CorsSettings>();

    if (corsSettings != null && !string.IsNullOrEmpty(corsSettings.PolicyName))
    {
        app.UseCors(corsSettings.PolicyName);
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => { options.AddPreferredSecuritySchemes("Bearer"); });
}

app.UseHttpsRedirection();

if (rateLimitSettings.Enabled)
{
    app.UseRateLimiter();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
   .RequireRateLimiting("api-limit");

app.Run();