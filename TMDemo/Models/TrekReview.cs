﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMDemo.Models
{
    public class TrekReview
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public string UserId { get; set; } 

        [Required]
        public int TrekId { get; set; } 


        public string? Comment { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
        [ForeignKey("UserId")]
        public UserDetail? User { get; set; }

        [ForeignKey("TrekId")]
        public Trek? Trek { get; set; }
    }
}
