﻿namespace TrekMasters.Repository
{
    public interface IAdminRepository
    {
        Task<List<Trek>> GetAllTreksAsync();
        
        Task AddTrekAsync(Trek trek);
        Task AddTrekPlanAsync(TrekPlan trekPlan);
        Task AddAvailabilityAsync(Availability availability);

        Task<List<TrekAvailabilityViewModel>> GetTrekBookingsAsync(int trekId);
        Task<List<UserViewModel>> GetUsersForAvailabilityAsync(int availabilityId);
        Availability GetAvailabilityById(int availabilityId);
        Task<bool> AvailabilityExistsAsync(int trekId);
        Task<List<NotificationRequest>> GetNotificationRequestsAsync(int trekId);
        Task RemoveNotificationRequests(List<NotificationRequest> notificationRequests);
        Task<Availability> GetConflictingAvailabilityAsync(int trekId, DateTime startDate, DateTime endDate);
        Task<List<int>> GetBookingIdsByUserIdAsync(string userId);
        Task<IEnumerable<TrekParticipant>> GetParticipantsByBookingIdAsync(int bookingId);
        Task<Booking> GetBookingByIdAsync(int bookingId);
    }
}


