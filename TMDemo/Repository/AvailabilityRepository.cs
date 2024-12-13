using Microsoft.EntityFrameworkCore;

namespace TrekMasters.Repository
{
    public class AvailabilityRepository : Repository,IAvailabilityRepository
    {
        private readonly AppDbContext _context;
        public AvailabilityRepository(AppDbContext context): base(context) 
        {
           _context = context;
        }
        public async Task<List<Availability>> GetPastAvailabilitiesAsync()
        {
            return await _context.Availabilities
                .Where(a => a.StartDate < DateTime.Now)
                .ToListAsync();
        }
    }
}
