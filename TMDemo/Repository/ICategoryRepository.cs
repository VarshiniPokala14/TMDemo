namespace TrekMasters.Repository
{
    public interface ICategoryRepository
    {
        Task<List<Trek>> GetAllTreksWithDetailsAsync(); 
        Task<List<Availability>> GetAllAvailabilitiesAsync(); 
    }

}
