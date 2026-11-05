namespace Core.DTOs
{
    public record HealthCheckItemDto(
        string Name,
        string Status,
        string? Description,
        string Duration
    );

    public record HealthResponseDto(
        string Status,
        string TotalDuration,
        IEnumerable<HealthCheckItemDto> Checks
    );
}