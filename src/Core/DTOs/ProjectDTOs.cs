using System.ComponentModel.DataAnnotations;

namespace Core.DTOs
{
    public record CreateProjectDTO
    (
        [Required(ErrorMessage = "OrganisationId is required.")]
        Guid OrganisationId,
        [Required(ErrorMessage = "Project name is required.")]
        [StringLength(150,MinimumLength = 2,ErrorMessage = "Project name must be between 2 and 150 characters.")]
        string Name,
        [RegularExpression("^(planned|in_progress|completed|cancelled)$",ErrorMessage = "Status must be 'planned', 'in_progress', 'completed', or 'cancelled'.")]
        string? Status="planned",
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        string? Description=null
    );

    public record UpdateProjectDTO
    (
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Project name must be between 2 and 150 characters.")]
        string? Name,
        [RegularExpression("^(planned|in_progress|completed|cancelled)$", 
        ErrorMessage = "Status must be 'planned', 'in_progress', 'completed', or 'cancelled'.")]
        string? Status,
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        string? Description
    );

    public record ProjectResponseDTO
    (
        Guid Id,
        Guid OrganisationId,
        string Name,
        string Status,
        string? Description,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );

    public record ProjectParams
    (
        int Page=1,
        int Limit=10,
        string? SortBy="createdAt",
        string? OrderBy="desc",
        string? Status=null,
        string? Name=null,
        Guid? OrganisationId=null
    )
    {
        public string EffectiveSortBy => !string.IsNullOrWhiteSpace(OrderBy) ? OrderBy : (SortBy ?? "createdAt");
    }
}