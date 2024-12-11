namespace TrekMasters.Service
{
    public interface IProfileService
    {
        Task<UserDetail> GetUserAsync(string userId);
        Task<bool> UpdateUserAsync(UserDetail user);
        Task<EmergencyContact> GetEmergencyContactAsync(string userId);
        Task AddOrUpdateEmergencyContactAsync(string userId, EmergencyContact emergencyContact);
        Task<MyBookingsViewModel> GetBookingsByUserIdAsync(string userId);

	}
}
