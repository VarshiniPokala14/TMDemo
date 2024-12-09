using Microsoft.Extensions.Caching.Memory;

namespace TMDemo.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        public CategoryRepository(AppDbContext context,IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<Trek>> GetAllTreksWithDetailsAsync()
        {
            return await _context.Treks
                .Include(t => t.Availabilities)
                .Include(t => t.TrekReviews)
                .Include(t => t.TrekPlans)
                .ToListAsync();
        }


        public async Task<List<Availability>> GetAllAvailabilitiesAsync()
        {
            string _cacheKey = "Availabilities:All";
            if(!_cache.TryGetValue(_cacheKey, out List<Availability> availabilities)) { 
                availabilities = await _context.Availabilities
                .Include(a => a.Trek)
                .ToListAsync();
                _cache.Set(_cacheKey, availabilities,TimeSpan.FromMinutes(30));
            }
            return availabilities;
        }
    }
}
