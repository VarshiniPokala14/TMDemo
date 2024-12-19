namespace TrekMasters.Repository
{
    public interface ITrekRepository:IRepository    {
        
        Task<List<Trek>> GetUpcomingTreksAsync(DateTime startDate, DateTime endDate);
        Task<byte[]> GetTrekImageAsync(int trekId);
        
        Task<Trek> GetTrekDetailsAsync(int trekId);
        Task<int> GetRemainingSlotsAsync(int trekId, DateTime startDate);
        Task<Booking> GetUserBookingAsync(string userId, int trekId);
        Task<bool> ReviewExistsAsync(string userId, int trekId);
        Task<List<Trek>> SearchTreksAsync(string searchString);
        Task<NotificationRequest> GetNotificationRequestAsync(int trekId, string email);
        Task<string> GetAdminUserIdAsync();



    }
}
