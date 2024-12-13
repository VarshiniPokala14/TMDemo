namespace TrekMasters.Repository
{
    public interface IUserRepository : IRepository
    {
        Task<UserDetail> GetUserAsync(string userId);
        Task<EmergencyContact> GetEmergencyContactAsync(string userId);
        Task<List<Booking>> GetBookingsByUserIdAsync(string userId);
        string GetCurrentUserId();
        Task<UserDetail> GetUserByIdAsync(string userId);
    }
}
