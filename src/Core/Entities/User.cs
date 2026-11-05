using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities
{
    [Table("users")]
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        public Guid Id{get;set;}=Guid.NewGuid();
        [Required]
        public Guid OrganisationId {get;set;}
        [ForeignKey(nameof(OrganisationId))]
        public Organisation Organisaion {get;set;}=null!;
        [Required]
        [MaxLength(100)]
        public string FullName {get;set;}=string.Empty;
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email {get;set;}=string.Empty;
        [Required]
        public string PasswordHash {get;set;}=string.Empty;
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "Member"; 
        public DateTime CreatedAt {get;set;}=DateTime.UtcNow;
    }
}
