namespace TMDemo.ViewModel
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
        public string DifficultyLevel { get; set; }

        [Required]
        public int DurationDays { get; set; }

        [Required]
        public decimal HighAltitude { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public List<string> SelectedSeasons { get; set; } = new List<string>();


        public IFormFile TrekImgFile { get; set; }

    }
}
