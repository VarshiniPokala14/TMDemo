namespace TrekMasters.Repository
{
    public interface IAvailabilityRepository : IRepository
    {
        Task<List<Availability>> GetPastAvailabilitiesAsync();

    }
}
