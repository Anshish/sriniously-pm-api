using System.ComponentModel.DataAnnotations;

namespace Core.DTOs
{
    public record CreateOrganisationDto(
        [Required(ErrorMessage = "Organisation name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Organisation name must be between 2 and 100 characters.")]
        string Name,

        [RegularExpression("^(active|archived)$", ErrorMessage = "Status must be either 'active' or 'archived'.")]
        string? Status,

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        string? Description
    );

    public record UpdateOrganisationDto(
        [Required(ErrorMessage = "Organisation name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Organisation name must be between 2 and 100 characters.")]
        string Name,

        [RegularExpression("^(active|archived)$", ErrorMessage = "Status must be either 'active' or 'archived'.")]
        string? Status,

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        string? Description
    );

    public record OrganisationResponseDto(
        Guid Id,
        string? Name,
        string? Status,
        string? Description,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}