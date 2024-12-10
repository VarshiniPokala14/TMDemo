namespace TMDemo.Repository
{
    public interface ITrekRepository
    {
        
        Task<List<Trek>> GetUpcomingTreksAsync(DateTime startDate, DateTime endDate);
        Task<byte[]> GetTrekImageAsync(int trekId);
        
        Task<Trek> GetTrekDetailsAsync(int trekId);
        Task<List<Availability>> GetPastAvailabilitiesAsync();
        Task<int> GetRemainingSlotsAsync(int trekId, DateTime startDate);
        Task<bool> RemovePastAvailabilitiesAsync(List<Availability> pastAvailabilities);
        Task<Booking> GetUserBookingAsync(string userId, int trekId);
        Task<bool> ReviewExistsAsync(string userId, int trekId);
        Task AddReviewAsync(TrekReview review);
        Task<List<Trek>> SearchTreksAsync(string searchString);
        Task<NotificationRequest> GetNotificationRequestAsync(int trekId, string email);
        Task AddNotificationRequest(NotificationRequest notificationRequest);



    }
}
