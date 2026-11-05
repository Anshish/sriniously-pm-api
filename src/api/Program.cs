using System.Text.Json;
using System.Threading.RateLimiting;
using api.Health;
using api.Middlewares;
using Api.Middleware;
using Core.DTOs;
using Core.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options=>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration=builder.Configuration.GetConnectionString("Redis");
    options.InstanceName="BackendPrinciples";
});

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("PostgreSQL")
    .AddCheck<RedisHealthCheck>("Redis");

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode=StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("fixed-window", opt =>
    {
        opt.PermitLimit=10;
        opt.Window=TimeSpan.FromSeconds(10);
        opt.QueueProcessingOrder=QueueProcessingOrder.OldestFirst;
        opt.QueueLimit=0;
    });
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType="application/json";
        var correlationId=context.HttpContext.Items["CorrelationId"]?.ToString()??context.HttpContext.TraceIdentifier;
        var problemDetails = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc6585#section-4",
            Title = "Too Many Requests",
            Status = StatusCodes.Status429TooManyRequests,
            Detail = "Rate limit exceeded. Maximum 10 requests allowed every 10 seconds.",
            Instance = context.HttpContext.Request.Path
        };
        problemDetails.Extensions["correlationId"] = correlationId;

        var json = JsonSerializer.Serialize(problemDetails);
        await context.HttpContext.Response.WriteAsync(json, cancellationToken: token);
    };
});

builder.Services.AddSingleton<ICacheService,RedisCacheService>();
builder.Services.AddScoped<OrganisationService>();
builder.Services.AddScoped<IOrganisationService>(provider=>
    new CachedOrganisationService(
        provider.GetRequiredService<OrganisationService>(),
        provider.GetRequiredService<ICacheService>()
    )
);
builder.Services.AddScoped<IProjectService,ProjectService>();
builder.Services.AddScoped<ITaskService,TaskService>();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseRateLimiter();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var checks = report.Entries.Select(e => new HealthCheckItemDto(
            Name: e.Key,
            Status: e.Value.Status.ToString(),
            Description: e.Value.Description,
            Duration: $"{e.Value.Duration.TotalMilliseconds:0.00}ms"
        ));

        var responseDto = new HealthResponseDto(
            Status: report.Status.ToString(),
            TotalDuration: $"{report.TotalDuration.TotalMilliseconds:0.00}ms",
            Checks: checks
        );

        var json = JsonSerializer.Serialize(responseDto, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
});
app.MapGet("/", () => "Progect Management Is Ready");
app.MapControllers().RequireRateLimiting("fixed-window");

app.Run();
