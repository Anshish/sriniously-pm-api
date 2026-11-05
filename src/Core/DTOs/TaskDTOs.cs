using System.ComponentModel.DataAnnotations;
using Core.DTOs;

namespace Core.DTOs
{
    public record CreateTaskDTO(
        [Required(ErrorMessage = "ProjectId is required.")]
        Guid ProjectId,

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200,MinimumLength = 2,ErrorMessage = "Title must be between 2 and 200 characters")]
        string Title,

        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        string? Description,

        [RegularExpression("^(todo|in_progress|completed|cancelled)$", 
            ErrorMessage = "Status must be 'todo', 'in_progress', 'completed', or 'cancelled'.")]
        string? Status,

        [RegularExpression("^(low|medium|high|urgent)$", 
            ErrorMessage = "Priority must be 'low', 'medium', 'high', or 'urgent'.")]
        string? Priority,

        DateTime? DueDate
    );

    public record UpdateTaskDTO(
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters.")]
        string? Title,

        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        string? Description,

        [RegularExpression("^(todo|in_progress|completed|cancelled)$", 
            ErrorMessage = "Status must be 'todo', 'in_progress', 'completed', or 'cancelled'.")]
        string? Status,

        [RegularExpression("^(low|medium|high|urgent)$", 
            ErrorMessage = "Priority must be 'low', 'medium', 'high', or 'urgent'.")]
        string? Priority,

        DateTime? DueDate
    );

    public record TaskResponseDTO(
        Guid Id,
        Guid ProjectId,
        string Title,
        string? Description,
        string Status,
        string Priority,
        DateTime? DueDate,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );

    public record TaskParams : PaginationParams
    {
        public Guid? ProjectId {get;set;}
        public string? Priority {get;set;}
    }
}