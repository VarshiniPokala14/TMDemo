namespace TrekMasters.Service
{
    public interface ICategoryService
    {
        Task<List<GroupedTreksByCategory>> GetTreksGroupedBySeasonAsync();
        Task<List<GroupedTreksByCategory>> GetTreksGroupedByDifficultyAsync();
        Task<List<GroupedTreksByCategory>> GetTreksGroupedByRegionAsync();
        Task<List<GroupedTreksByCategory>> GetTreksGroupedByDurationAsync();
        Task<List<GroupedTreksByCategory>> GetTreksGroupedByMonthAsync();
        Task<GroupedTreksByCategory> FilterTreksByCategoryAsync(string categoryType, string filterValue);

    }
}
