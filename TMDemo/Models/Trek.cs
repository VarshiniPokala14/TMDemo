﻿namespace TrekMasters.Models
{
    public class Trek
    {
        [Key]
        public int TrekId { get; set; }

        [Required, MaxLength(50)]
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
        public decimal HighAltitude { get; set; } 
        [Required]
        public List<string> Season { get; set; }
        [Required]
        public decimal Price { get; set; } 
        public byte[] TrekImg {  get; set; }


        
        public ICollection<TrekPlan>? TrekPlans { get; set; }
        public ICollection<Availability>? Availabilities { get; set; }
        public ICollection<TrekReview>? TrekReviews { get; set; }
       public ICollection<Booking>? Bookings { get; set; }
    }
}
