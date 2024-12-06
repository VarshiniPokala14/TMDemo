namespace TMDemo.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
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
            return await _context.Availabilities
                .Include(a => a.Trek)
                .ToListAsync();
        }
    }
}
