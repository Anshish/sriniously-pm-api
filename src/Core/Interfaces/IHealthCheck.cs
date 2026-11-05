using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Core.Interfaces
{
    public interface IHealthCheck
    {
        Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default);
    }    
}
