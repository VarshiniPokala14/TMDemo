namespace TrekMasters.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public string SenderId { get; set; } // The user who sends the notification

        [Required]
        public string RecipientId { get; set; } // The user who receives the notification

        [Required]
        public string Message { get; set; } // Notification message

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false; // Track if the notification is read
    }

}
