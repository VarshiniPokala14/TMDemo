namespace TrekMasters.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
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

        public void AddBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();
        }

        public void UpdateBooking(Booking booking)
        {
            _context.Bookings.Update(booking);
            _context.SaveChanges();
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


    }

}
