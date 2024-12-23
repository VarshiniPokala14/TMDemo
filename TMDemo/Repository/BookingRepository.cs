using TrekMasters.Models;

namespace TrekMasters.Repository
{
    public class BookingRepository :Repository, IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context):base(context) 
        {
            _context = context;
        }

        public Trek GetTrekById(int trekId)
        {
            return _context.Treks.FirstOrDefault(t => t.TrekId == trekId);
        }

        public Booking GetBookingById(int bookingId)
        {
            return _context.Bookings.Include(b => b.Trek).FirstOrDefault(b => b.BookingId == bookingId);
        }
        public List<DateTime> GetAvailableDates(int trekId)
        {
            return _context.Availabilities
                           .Where(a => a.TrekId == trekId && a.StartDate > DateTime.Now)
                           .Select(a => a.StartDate)
                           .Distinct()
                           .OrderBy(d => d)
                           .ToList();
        }
        public async Task<List<Booking>> GetOverlappingBookingsAsync(string email, DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .Include(b => b.Trek)
              
                .Where(b => b.TrekParticipants.Any(p => p.Email == email)
                            && b.TrekStartDate < endDate
                            && b.TrekStartDate.AddDays(b.Trek.DurationDays) > startDate && b.PaymentSuccess==true)
                .ToListAsync();
        }
        public async Task<List<Booking>> GetTotalBookingsAsync()
        {
            return await _context.Bookings.Include(t => t.Trek).Where(b => b.PaymentSuccess == true).ToListAsync();
        }
        


    }

}
