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
        [FutureOrTodayDate(ErrorMessage = "EndDate shouldn't be past date")]
        public DateTime EndDate { get; set; }
        [Required]
        public string Month { get; set; }

        [Required(ErrorMessage = "Max Group Size is required.")]
        [Range(10, 30, ErrorMessage = "Max Group Size must be between 10 and 30.")]
        public int MaxGroupSize { get; set; }

        public Trek? Trek { get; set; }
    }
}
