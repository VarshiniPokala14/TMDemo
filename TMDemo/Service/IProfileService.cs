namespace TMDemo.Service
{
    public interface IProfileService
    {
        Task<UserDetail> GetUserAsync(string userId);
        Task<bool> UpdateUserAsync(UserDetail user);
        Task<EmergencyContact> GetEmergencyContactAsync(string userId);
        Task AddOrUpdateEmergencyContactAsync(string userId, EmergencyContact emergencyContact);
        Task<List<Booking>> GetBookingsByUserIdAsync(string userId);
    }
}
