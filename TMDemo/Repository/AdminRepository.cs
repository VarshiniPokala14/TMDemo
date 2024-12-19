namespace TrekMasters.Repository
{
    public class AdminRepository : Repository, IAdminRepository 
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context):base(context)
        {
            _context = context;
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
                        .Where(b => b.TrekId == trekId && b.TrekStartDate == a.StartDate && b.PaymentSuccess == true)
                        .Sum(b => b.NumberOfPeople) - _context.Bookings
                        .Where(b => b.TrekId == trekId && b.TrekStartDate == a.StartDate && (b.IsCancelled == true) && b.PaymentSuccess==true)
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
                .Where(b => b.TrekStartDate == startDate && (b.IsCancelled == false || b.IsCancelled == null) && b.PaymentSuccess==true)
                .Select(b => new UserViewModel
                {
                    UserId=b.UserId,
                    UserName = b.User.FirstName,
                    Email = b.User.Email,
                    NumberOfPeople = b.NumberOfPeople,
                    BookingId = b.BookingId,
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
                .AnyAsync(a => a.TrekId == trekId); 
        }


        public async Task<List<NotificationRequest>> GetNotificationRequestsAsync(int trekId)
        {
            return await _context.NotificationRequests
                .Where(nr => nr.TrekId == trekId)
                .ToListAsync();
        }

        
        public async Task<Availability> GetConflictingAvailabilityAsync(int trekId, DateTime startDate, DateTime endDate)
        {
            return await _context.Availabilities
                .FirstOrDefaultAsync(a => a.TrekId == trekId &&
                                          a.StartDate <= endDate &&
                                          a.EndDate >= startDate);
        }
        public async Task<List<int>> GetBookingIdsByUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Select(b => b.BookingId)
                .ToListAsync();
        }
        public async Task<IEnumerable<TrekParticipant>> GetParticipantsByBookingIdAsync(int bookingId)
        {
            return await _context.TrekParticipants
                .Where(tp => tp.BookingId == bookingId)
                .ToListAsync();
        }
        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            if (bookingId <= 0)
            {
                throw new ArgumentException("Invalid booking ID", nameof(bookingId));
            }

            return await _context.Bookings
                .Include(b => b.User) 
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }
        public async Task<Availability> GetAvailabilityByIdAsync(int availabilityId)
        {
            return await _context.Availabilities
                .Include(a => a.Trek)
                .FirstOrDefaultAsync(a => a.AvailabilityId == availabilityId);
        }

        public async Task<List<Booking>> GetBookingsByAvailabilityIdAsync(int availabilityId)
        {
            var startDate = await _context.Availabilities
                .Where(a => a.AvailabilityId == availabilityId)
                .Select(a => a.StartDate)
                .FirstOrDefaultAsync();

            return await _context.Bookings
                
                .Where(b => b.TrekStartDate == startDate && (b.IsCancelled == false || b.IsCancelled == null) && b.PaymentSuccess==true)
                .Include(u=>u.User)
                .Include(tp=>tp.TrekParticipants)
                .ToListAsync();
        }

        //public void UpdateBooking(Booking booking)
        //{
        //    _context.Bookings.Update(booking);
        //}

        //public async Task SaveChangesAsync()
        //{
        //    await _context.SaveChangesAsync();
        //}


    }

}
