namespace TMDemo.Service
{
    public interface IAdminService
    {
        Task<int> AddTrekAsync(AddTrekViewModel model);
        Task AddTrekPlanAsync(TrekPlanViewModel model);
        //Task AddAvailabilityAsync(Availability availability);
        Task<List<Trek>> GetAllTreksAsync();
        Task<List<Availability>> GetAllAvailabilitiesAsync();
        Availability GetAvailabilityById(int availabilityId);
        Task<List<TrekAvailabilityViewModel>> GetTrekBookingsAsync(int trekId);
        Task<List<UserViewModel>> GetUsersForAvailabilityAsync(int availabilityId);
        Task<Trek> GetTrekByIdAsync(int trekId); // Method to get a specific trek
        
        Task AddAvailabilityAsync(int trekId, DateTime startDate, DateTime endDate,string month,int maxGroup);
    }

}
