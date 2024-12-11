namespace TrekMasters.Models
{
    public class TrekPlan
    {
        [Key]
        public int PlanId { get; set; }

        [Required]
        public int TrekId { get; set; }  

        [Required]
        public int Day { get; set; }  

        [Required]
        public string ActivityDescription { get; set; }  

        
        public Trek Trek { get; set; }
    }
}

