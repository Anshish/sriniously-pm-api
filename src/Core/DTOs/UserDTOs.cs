namespace Core.DTOs
{
    public record RegisterRequestDTO(
        Guid OrganisationId,
        string FullName,
        string Email,
        string Password
    );
    public record LoginRequestDTO(
        string FullName,
        string Password
    );
    public record AuthResponseDTO(
        Guid UserId,
        string FullName,
        string Email,
        Guid OrganisationId,
        string Token
    );
}