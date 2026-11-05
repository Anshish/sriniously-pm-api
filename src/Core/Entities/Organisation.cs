using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    [Table("organisations")]
    public class Organisation
    {
        [Key]
        public Guid Id {get;set;}=Guid.NewGuid();
        [Required]
        [MaxLength(255)]
        public string Name {get;set;}=string.Empty;
        [MaxLength(50)]
        public string Status {get;set;}="active";
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Project> Projects {get;set;}=new List<Project>();
    }
}