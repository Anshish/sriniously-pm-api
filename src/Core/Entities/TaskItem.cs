using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities
{
    [Table("tasks")]
    [Index(nameof(ProjectId))]
    [Index(nameof(Status))]
    public class TaskItem
    {
        [Key]
        public Guid Id {get;set;}=Guid.NewGuid();

        [Required]
        public Guid ProjectId {get;set;}

        [ForeignKey(nameof(ProjectId))]
        public Project Project {get;set;}=null;
        
        [Required]
        [MaxLength(200)]
        public string Title {get;set;}=string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "todo";

        [Required]
        [MaxLength(50)]
        public string Priority { get; set; } = "medium";

        public DateTime? DueDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}