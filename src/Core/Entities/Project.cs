using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    [Table("projects")]
    public class Project
    {
        [Key]
        public Guid Id {get;set;}=Guid.NewGuid();
        public Guid OrganisationId {get;set;}
        [Required]
        [MaxLength(255)]
        public string Name {get;set;}=string.Empty;
        [MaxLength(50)]
        public string Status {get;set;}="planned";
        public string? Description {get;set;}
        public DateTime CreatedAt {get;set;}=DateTime.UtcNow;
        public DateTime UpdatedAt {get;set;}=DateTime.UtcNow;

        [ForeignKey(nameof(OrganisationId))]
        public Organisation Organisation {get;set;}=null!;
    }
}