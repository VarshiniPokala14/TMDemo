namespace TMDemo.Service
{
    public interface ITrekService
    {
        Task<List<Trek>> GetAllTreksAsync();
        Task<List<Trek>> GetUpcomingTreksAsync();
        Task<TrekDetailsViewModel> GetTrekDetailsAsync(int trekId, string userId);
        Task CleanupPastAvailabilitiesAsync();
        Task<byte[]> GetTrekImageAsync(int trekId);
        Task<string> AddReviewAsync( int trekId, string reviewText);
        Task<List<Trek>> SearchTreksAsync(string searchString);

    }
}
