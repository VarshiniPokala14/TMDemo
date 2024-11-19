using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TMDemo.Models
{
    public class EmergencyContact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string ContactName { get; set; }

        [Required]
        [MaxLength(15)]
        public string ContactNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string Relation { get; set; }

        [ForeignKey("UserDetail")]
        public string UserId { get; set; } // FK should match `IdentityUser` key type (string by default)

        public UserDetail? UserDetail { get; set; } // Navigation property
    }
}
