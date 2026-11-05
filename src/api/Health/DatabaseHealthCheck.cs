using Infrastructure.Persistence;
using Core.DTOs;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using FrameworkHealthCheck = Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck;

namespace api.Health
{
    public class DatabaseHealthCheck(IServiceScopeFactory scopeFactory) : FrameworkHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,CancellationToken ct = default)
        {
            try
            {
                using var scope=scopeFactory.CreateScope();
                var db=scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var canConnect=await db.Database.CanConnectAsync(ct);
                return canConnect
                    ?HealthCheckResult.Healthy("PostgreSQL is reachable")
                    :HealthCheckResult.Unhealthy("Unable to connect to PostgreSQL");
            }catch(Exception ex)
            {
                return HealthCheckResult.Unhealthy($"PostgreSQL check failed: {ex.Message}");
            }
        }
    }
}
