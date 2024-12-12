namespace TrekMasters.Service
{
    public interface IAdminService
    {
        Task<int> AddTrekAsync(AddTrekViewModel model);
        Task AddTrekPlanAsync(TrekPlanViewModel model);
        Task<List<Trek>> GetAllTreksAsync();
        Task<List<Availability>> GetAllAvailabilitiesAsync();
        Availability GetAvailabilityById(int availabilityId);
        Task<List<TrekAvailabilityViewModel>> GetTrekBookingsAsync(int trekId);
        Task<List<UserViewModel>> GetUsersForAvailabilityAsync(int availabilityId);
        Task<Trek> GetTrekByIdAsync(int trekId); 
        
        Task AddAvailabilityAsync(int trekId, DateTime startDate, DateTime endDate,string month,int maxGroup);
        Task<bool> CheckAvailabilityConflictAsync(int trekId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<TrekParticipant>> GetParticipantsForBookingAsync(int bookingId);
        Task<Booking> GetBookingByIdAsync(int bookingId);
    }

}
