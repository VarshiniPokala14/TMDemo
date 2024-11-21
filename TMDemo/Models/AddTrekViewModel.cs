using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TMDemo
{
    public class AddTrekViewModel
    {
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
        public decimal Price { get; set; }

        [Required]
        public List<string> SelectedSeasons { get; set; } = new List<string>(); // Selected seasons


        public IFormFile TrekImgFile { get; set; } // File for the image
    }
}
