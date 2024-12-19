namespace TrekMasters.Service
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string senderId, string recipientId, string message);
        Task<List<Notification>> GetNotificationsForUserAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task<List<Notification>> GetUnreadNotificationsForUserAsync(string userId);
    }
}
