using System.ComponentModel.DataAnnotations;

namespace TMDemo.Models
{
    public class Availability
    {
        [Key]
        public int AvailabilityId { get; set; }

        [Required]
        public int TrekId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } 

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public string Month { get; set; }

        [Required]
        public int MaxGroupSize { get; set; }

        
        public Trek? Trek { get; set; }
    }
}
