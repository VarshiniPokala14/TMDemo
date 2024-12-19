namespace TrekMasters.Repository
{
    public interface IAdminRepository : IRepository
    {
        

        Task<List<TrekAvailabilityViewModel>> GetTrekBookingsAsync(int trekId);
        Task<List<UserViewModel>> GetUsersForAvailabilityAsync(int availabilityId);
        Availability GetAvailabilityById(int availabilityId);
        Task<bool> AvailabilityExistsAsync(int trekId);
        Task<List<NotificationRequest>> GetNotificationRequestsAsync(int trekId);
        Task<Availability> GetConflictingAvailabilityAsync(int trekId, DateTime startDate, DateTime endDate);
        Task<List<int>> GetBookingIdsByUserIdAsync(string userId);
        Task<IEnumerable<TrekParticipant>> GetParticipantsByBookingIdAsync(int bookingId);
        Task<Booking> GetBookingByIdAsync(int bookingId);
        Task<Availability> GetAvailabilityByIdAsync(int availabilityId);
        Task<List<Booking>> GetBookingsByAvailabilityIdAsync(int availabilityId);
    }
}


