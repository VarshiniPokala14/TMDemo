namespace TrekMasters.Repository
{
    public interface IUserRepository
    {
        Task<UserDetail> GetUserAsync(string userId);
        Task<bool> UpdateUserAsync(UserDetail user);
        Task<EmergencyContact> GetEmergencyContactAsync(string userId);
        Task AddEmergencyContactAsync(EmergencyContact emergencyContact);
        Task UpdateEmergencyContactAsync(EmergencyContact emergencyContact);
        Task<List<Booking>> GetBookingsByUserIdAsync(string userId);
        string GetCurrentUserId();
        Task<UserDetail> GetUserByIdAsync(string userId);
    }
}
