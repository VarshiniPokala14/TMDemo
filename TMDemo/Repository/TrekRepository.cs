namespace TrekMasters.Repository
{
    public class TrekRepository : ITrekRepository
    {
        private readonly AppDbContext _context;

        public TrekRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Trek>> GetUpcomingTreksAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Treks
                .Include(t => t.Availabilities)
                .Where(t => t.Availabilities.Any(a => a.StartDate >= startDate && a.StartDate <= endDate))
                .ToListAsync();
        }

        
        public async Task<byte[]> GetTrekImageAsync(int trekId)
        {
            return await _context.Treks
                .Where(t => t.TrekId == trekId)
                .Select(t => t.TrekImg)
                .FirstOrDefaultAsync();
        }
        public async Task<Trek> GetTrekDetailsAsync(int trekId)
        {
            return await _context.Treks
                .Include(t => t.Availabilities)
                .Include(t => t.TrekReviews)
                    .ThenInclude(r => r.User)
                .Include(t => t.Bookings)
                    .ThenInclude(b => b.User)
                .Include(t => t.TrekPlans)
                .FirstOrDefaultAsync(t => t.TrekId == trekId);
        }

        public async Task<List<Availability>> GetPastAvailabilitiesAsync()
        {
            return await _context.Availabilities
                .Where(a => a.StartDate < DateTime.Now)
                .ToListAsync();
        }

        public async Task<int> GetRemainingSlotsAsync(int trekId, DateTime startDate)
        {
            var totalSlots = await _context.Availabilities
                .Where(a => a.TrekId == trekId && a.StartDate == startDate)
                .Select(a => a.MaxGroupSize)
                .FirstOrDefaultAsync();

            var totalBookedSlots = await _context.Bookings
                .Where(b => b.TrekId == trekId && b.TrekStartDate == startDate)
                .SumAsync(b => b.NumberOfPeople);

            var totalCancelledSlots = await _context.Bookings
                .Where(b => b.TrekId == trekId && b.TrekStartDate == startDate && b.IsCancelled==true)
                .SumAsync(b => b.NumberOfPeople);

            return totalSlots - (totalBookedSlots - totalCancelledSlots);
        }

        public async Task<bool> RemovePastAvailabilitiesAsync(List<Availability> pastAvailabilities)
        {
            _context.Availabilities.RemoveRange(pastAvailabilities);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Booking> GetUserBookingAsync(string userId, int trekId)
        {
            return await _context.Bookings
                .Include(t => t.Trek)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.TrekId == trekId &&
                                          (b.IsCancelled == false || b.IsCancelled == null));
        }

        public async Task<bool> ReviewExistsAsync(string userId, int trekId)
        {
            return await _context.TrekReviews
                .AnyAsync(r => r.UserId == userId && r.TrekId == trekId);
        }

        public async Task AddReviewAsync(TrekReview review)
        {
            _context.TrekReviews.Add(review);
            await _context.SaveChangesAsync();
        }
        public async Task AddNotificationRequest(NotificationRequest notificationRequest)
        {
             _context.NotificationRequests.AddAsync(notificationRequest);
            await _context.SaveChangesAsync();

        }

        public async Task<NotificationRequest> GetNotificationRequestAsync(int trekId, string email)
        {
            return await _context.NotificationRequests
                                  .FirstOrDefaultAsync(nr => nr.TrekId == trekId && nr.Email == email);
        }
        public async Task<List<Trek>> SearchTreksAsync(string searchString)
        {
            var query = _context.Treks.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(t => t.Name.ToUpper().Contains(searchString.ToUpper()));
            }

            return await query.ToListAsync();
        }
    }
}
