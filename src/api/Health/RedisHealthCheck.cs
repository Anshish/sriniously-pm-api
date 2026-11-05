using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using FrameworkHealthCheck = Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck;

namespace api.Health
{
    public class RedisHealthCheck(ICacheService cache) : FrameworkHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,CancellationToken ct = default)
        {
            try
            {
                await cache.GetAsync<string>("__health_ping__",ct);
                return HealthCheckResult.Healthy("Redis is reachable.");
            }catch(Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Redis check failed: {ex.Message}");
            }
        }
    }
}
