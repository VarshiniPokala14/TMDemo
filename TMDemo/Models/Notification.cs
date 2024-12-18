namespace TrekMasters.Models
{
    public class Notification
    {
        public int Id { get; set; } 
        public string Message { get; set; } 
        public int TrekId { get; set; } 
        public string Email {  get; set; }
        public List<NotificationRequest> NotificationRequests { get; set; } 
    }
}
