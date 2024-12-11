namespace TrekMasters.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Trek>> GetAllTreksAsync()
        {
            return await _context.Treks.ToListAsync();
        }

       

        public async Task AddTrekAsync(Trek trek)
        {
            await _context.Treks.AddAsync(trek);
            await _context.SaveChangesAsync();
        }

        public async Task AddTrekPlanAsync(TrekPlan trekPlan)
        {
            await _context.TrekPlans.AddAsync(trekPlan);
            await _context.SaveChangesAsync();
        }

        public async Task AddAvailabilityAsync(Availability availability)
        {
             _context.Availabilities.AddAsync(availability);
            await _context.SaveChangesAsync();
        }

        

        public async Task<List<TrekAvailabilityViewModel>> GetTrekBookingsAsync(int trekId)
        {
            return await _context.Availabilities
                .Where(a => a.TrekId == trekId)
                .Select(a => new TrekAvailabilityViewModel
                {
                    AvailabilityId = a.AvailabilityId,
                    StartDate = a.StartDate,
                    MaxGroupSize = a.MaxGroupSize,
                    RemainingSlots = a.MaxGroupSize - (_context.Bookings
                        .Where(b => b.TrekId == trekId && b.TrekStartDate == a.StartDate)
                        .Sum(b => b.NumberOfPeople) - _context.Bookings
                        .Where(b => b.TrekId == trekId && b.TrekStartDate == a.StartDate && (b.IsCancelled == true))
                        .Sum(b => b.NumberOfPeople))
                })
                .ToListAsync();
        }

        public async Task<List<UserViewModel>> GetUsersForAvailabilityAsync(int availabilityId)
        {
            var startDate = await _context.Availabilities
                .Where(a => a.AvailabilityId == availabilityId)
                .Select(a => a.StartDate)
                .FirstOrDefaultAsync();

            return await _context.Bookings
                .Where(b => b.TrekStartDate == startDate && (b.IsCancelled == false || b.IsCancelled == null))
                .Select(b => new UserViewModel
                {
                    UserName = b.User.FirstName,
                    Email = b.User.Email,
                    NumberOfPeople = b.NumberOfPeople,
                    BookingDate = b.BookingDate
                })
                .ToListAsync();
        }
        public Availability GetAvailabilityById(int availability)
        {
            return _context.Availabilities.Include(t=>t.Trek)
                .FirstOrDefault(a=>a.AvailabilityId == availability);
        }
        public async Task<bool> AvailabilityExistsAsync(int trekId)
        {
            return await _context.Availabilities
                .AnyAsync(a => a.TrekId == trekId); // Use AnyAsync for condition-based checks
        }


        public async Task<List<NotificationRequest>> GetNotificationRequestsAsync(int trekId)
        {
            return await _context.NotificationRequests
                .Where(nr => nr.TrekId == trekId)
                .ToListAsync();
        }

        public async Task RemoveNotificationRequests(List<NotificationRequest> notificationRequests)
        {
            _context.NotificationRequests.RemoveRange(notificationRequests);
            await _context.SaveChangesAsync();
        }
        public async Task<Availability> GetConflictingAvailabilityAsync(int trekId, DateTime startDate, DateTime endDate)
        {
            return await _context.Availabilities
                .FirstOrDefaultAsync(a => a.TrekId == trekId &&
                                          a.StartDate <= endDate &&
                                          a.EndDate >= startDate);
        }

    }

}
