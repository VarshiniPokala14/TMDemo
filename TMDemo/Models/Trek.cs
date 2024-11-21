using System.ComponentModel.DataAnnotations;
namespace TMDemo.Models
{
    public class Trek
    {
        [Key]
        public int TrekId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Region { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string DifficultyLevel { get; set; } // Easy, Moderate, Difficult

        [Required]
        public int DurationDays { get; set; }

        [Required]
        public decimal HighAltitude { get; set; } // meters/feet
        [Required]
        public List<string> Season { get; set; }
        [Required]
        public decimal Price { get; set; } 
        public byte[] TrekImg {  get; set; }

        // Navigation Property: Links to Availability table
        public ICollection<Availability>? Availabilities { get; set; }
       
    }
}
