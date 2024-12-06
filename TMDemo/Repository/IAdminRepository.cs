namespace TMDemo.Repository
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
    }

}
