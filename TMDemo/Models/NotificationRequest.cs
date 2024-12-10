namespace TMDemo.Models
{
    public class NotificationRequest
    {
        [Required]
        public int NotificationRequestId { get; set; }
        [Required]
        public int TrekId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public DateTime RequestedOn { get; set; }=DateTime.UtcNow;
    }

}
