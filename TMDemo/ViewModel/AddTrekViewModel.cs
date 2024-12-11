namespace TrekMasters.ViewModel
{
    public class AddTrekViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
       
        public string Name { get; set; }

        [Required(ErrorMessage = "Region is required.")]
        public string Region { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Difficulty Level is required.")]
        public string DifficultyLevel { get; set; }

        [Required(ErrorMessage = "Duration (in days) is required.")]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days.")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "High Altitude is required.")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Altitude must be greater than 0.")]
        public decimal HighAltitude { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please select at least one season.")]
        public List<string> SelectedSeasons { get; set; } = new List<string>();

        [Required(ErrorMessage = "Trek image is required.")]
        [AllowedFileTypes(new[] { ".jpg", ".jpeg", ".png", ".gif" , ".webp"}, ErrorMessage = "Only image files (.jpg, .jpeg, .png, .gif, .webp) are allowed.")]
        public IFormFile TrekImgFile { get; set; }
    }
}
