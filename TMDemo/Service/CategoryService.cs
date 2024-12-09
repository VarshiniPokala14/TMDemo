using Microsoft.Extensions.Caching.Memory;
using TMDemo.Repository;

namespace TMDemo.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMemoryCache _memoryCache;

        private const string TreksCacheKey = "AllTreksWithDetails";
        private const string AvailabilitiesCacheKey = "AllAvailabilities";

        public CategoryService(ICategoryRepository categoryRepository, IMemoryCache memoryCache)
        {
            _categoryRepository = categoryRepository;
            _memoryCache = memoryCache;
        }

        public async Task<List<GroupedTreksByCategory>> GetTreksGroupedBySeasonAsync()
        {
            var treks = await GetTreksFromCacheOrDatabase();
            return treks
                .SelectMany(t => t.Season, (t, season) => new { t, season })
                .GroupBy(ts => ts.season)
                .Select(g => new GroupedTreksByCategory
                {
                    Category = g.Key,
                    Treks = g.Select(ts => ts.t).Distinct().ToList()
                })
                .OrderBy(g => g.Category)
                .ToList();
        }

        public async Task<List<GroupedTreksByCategory>> GetTreksGroupedByDifficultyAsync()
        {
            var treks = await GetTreksFromCacheOrDatabase();
            return treks
                .GroupBy(t => t.DifficultyLevel)
                .Select(g => new GroupedTreksByCategory
                {
                    Category = g.Key,
                    Treks = g.ToList()
                })
                .OrderBy(g => g.Category)
                .ToList();
        }

        public async Task<List<GroupedTreksByCategory>> GetTreksGroupedByRegionAsync()
        {
            var treks = await GetTreksFromCacheOrDatabase();
            return treks
                .GroupBy(t => t.Region)
                .Select(g => new GroupedTreksByCategory
                {
                    Category = g.Key,
                    Treks = g.ToList()
                })
                .OrderBy(g => g.Category)
                .ToList();
        }

        public async Task<List<GroupedTreksByCategory>> GetTreksGroupedByDurationAsync()
        {
            var treks = await GetTreksFromCacheOrDatabase();
            return treks
                .GroupBy(t => t.DurationDays)
                .Select(g => new GroupedTreksByCategory
                {
                    Category = $"{g.Key} Days",
                    Treks = g.ToList()
                })
                .OrderBy(g => int.Parse(g.Category.Split(' ')[0]))
                .ToList();
        }

        public async Task<List<GroupedTreksByCategory>> GetTreksGroupedByMonthAsync()
        {
            var availabilities = await GetAvailabilitiesFromCacheOrDatabase();
            var monthOrder = new List<string>
            {
                "January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"
            };

            return availabilities
                .GroupBy(a => a.Month)
                .Select(g => new GroupedTreksByCategory
                {
                    Category = g.Key,
                    Treks = g.Select(a => a.Trek).Distinct().ToList()
                })
                .OrderBy(g => monthOrder.IndexOf(g.Category))
                .ToList();
        }

        public async Task<GroupedTreksByCategory> FilterTreksByCategoryAsync(string categoryType, string filterValue)
        {
            var treks = await GetTreksFromCacheOrDatabase();

            var filteredTreks = categoryType.ToLower() switch
            {
                "season" => treks.Where(t => t.Season.Contains(filterValue)).ToList(),
                "difficulty" => treks.Where(t => t.DifficultyLevel == filterValue).ToList(),
                "region" => treks.Where(t => t.Region == filterValue).ToList(),
                "duration" => filterValue == "6"
                    ? treks.Where(t => t.DurationDays >= 6).ToList()
                    : int.TryParse(filterValue, out int days)
                        ? treks.Where(t => t.DurationDays == days).ToList()
                        : new List<Trek>(),
                "month" => (await GetAvailabilitiesFromCacheOrDatabase())
                    .Where(a => a.Month == filterValue)
                    .Select(a => a.Trek)
                    .Distinct()
                    .ToList(),
                _ => new List<Trek>()
            };

            return new GroupedTreksByCategory
            {
                Category = filterValue,
                Treks = filteredTreks
            };
        }

        // Cache methods for Treks and Availabilities
        private async Task<List<Trek>> GetTreksFromCacheOrDatabase()
        {
            if (!_memoryCache.TryGetValue(TreksCacheKey, out List<Trek> treks))
            {
                treks = await _categoryRepository.GetAllTreksWithDetailsAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)); // Adjust cache expiration as needed

                _memoryCache.Set(TreksCacheKey, treks, cacheOptions);
            }

            return treks;
        }

        private async Task<List<Availability>> GetAvailabilitiesFromCacheOrDatabase()
        {
            if (!_memoryCache.TryGetValue(AvailabilitiesCacheKey, out List<Availability> availabilities))
            {
                availabilities = await _categoryRepository.GetAllAvailabilitiesAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)); // Adjust cache expiration as needed

                _memoryCache.Set(AvailabilitiesCacheKey, availabilities, cacheOptions);
            }

            return availabilities;
        }
    }
}
