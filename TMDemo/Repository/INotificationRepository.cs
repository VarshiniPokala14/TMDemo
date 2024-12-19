////namespace TrekMasters.Repository
////{
////    public interface INotificationRepository
////    {
////        Task AddNotificationAsync(Notification notification);
////        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
////        Task MarkAsReadAsync(int notificationId);
////        Task DeleteNotificationAsync(int notificationId);
////    }
////}
namespace TrekMasters.Repository
{
    public interface INotificationRepository:IRepository
    {
        Task<List<Notification>> GetNotificationsForUserAsync(string userId);
        Notification GetNotificationById(int Id);
        Task<List<Notification>> GetUnreadNotificationsForUserAsync( string userId);
    }
}
