namespace TMDemo.Repository
{
    public interface ICategoryRepository
    {
        Task<List<Trek>> GetAllTreksWithDetailsAsync(); 
        Task<List<Availability>> GetAllAvailabilitiesAsync(); 
    }

}
