using System.ComponentModel.DataAnnotations;
using TMDemo.Validation;

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
        [FutureOrTodayDate(ErrorMessage="Date shouldn't be past date")]
        public DateTime StartDate { get; set; } 

        [Required]
        [DataType(DataType.Date)]
        [FutureOrTodayDate(ErrorMessage = "Date shouldn't be past date")]
        public DateTime EndDate { get; set; }
        [Required]
        public string Month { get; set; }

        [Required]
        public int MaxGroupSize { get; set; }

        
        public Trek? Trek { get; set; }
    }
}
