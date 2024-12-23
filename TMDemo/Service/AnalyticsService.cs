
namespace TrekMasters.Service
{
    public class AnalyticsService
    {
        private readonly AppDbContext _context;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITrekRepository _trekRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;
        public AnalyticsService(AppDbContext context, ICategoryRepository categoryRepository,ITrekRepository trekRepository, IUserRepository userRepository, IBookingRepository bookingRepository)
        {
            _context = context;
            _categoryRepository = categoryRepository;
            _trekRepository = trekRepository;
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<DashboardAnalyticsViewModel> GetDashboardDataAsync()
        {
            var treks = await _categoryRepository.GetAllTreksWithDetailsAsync();
            var totaltreks = treks.Count();
            var adminUserId = await _trekRepository.GetAdminUserIdAsync();
            var usersList = await _userRepository.GetUserDetailsAsync();
            var users = usersList.Where(u => u.Id != adminUserId)
                .Select(u => new UserDetails
                {
                    Name = u.FirstName,
                    Contact = u.PhoneNumber,
                    Email = u.Email
                }).ToList();
            var totalUsers = users.Count();
            var bookings = await _bookingRepository.GetTotalBookingsAsync();
            var totalBookings = bookings.Count();
            var completedTreks = bookings.Where(b => (b.TrekStartDate.AddDays(b.Trek.DurationDays - 1) <= DateTime.Now)).GroupBy(b => new { b.TrekId, b.TrekStartDate }).Count();
            var ongoingTrekscount = bookings
                    .Where(b => (b.TrekStartDate <= DateTime.Now)
                    && b.TrekStartDate.AddDays(b.Trek.DurationDays - 1) >= DateTime.Now && b.IsCancelled == false)
                    .GroupBy(b => new { b.TrekId, b.TrekStartDate })
                    .Count();

            var today = DateTime.Now.Date;
            var cancels = bookings.Where(b => b.IsCancelled == true).ToList();
            var totalCancels = bookings.Where(b => b.IsCancelled == true).Count();
            var finaltotal = bookings.Sum(b => b.TotalAmount);
            var extraAmount = bookings.Sum(b => b.ExtraAmount ?? 0);
            var refundAmount = bookings.Sum(b => b.RefundAmount ?? 0);
            var totalrevenue = finaltotal + extraAmount - refundAmount;

            var ongoingTreks = await _context.Bookings
                .Where(b => b.TrekStartDate <= today && b.TrekStartDate.AddDays(b.Trek.DurationDays - 1) >= today
                            && (b.IsCancelled == false || b.IsCancelled == null) && b.PaymentSuccess == true)
                .GroupBy(b => new { b.TrekId, b.Trek.Name, b.Trek.Region, b.Trek.DurationDays, b.TrekStartDate })
                .Select(group => new TrekBookingViewModel
                {
                    TrekName = group.Key.Name,
                    TrekRegion = group.Key.Region,
                    TrekDurationDays = group.Key.DurationDays,
                    BookedUsers = group.Select(b => new UserListViewModel
                    {
                        UserName = b.User.FirstName,
                        Email = b.User.Email,
                        NumberOfPeople = b.NumberOfPeople,
                        BookingId = b.BookingId,
                        TotalAmount = b.TotalAmount,
                        BookingDate = b.BookingDate
                    }).ToList()
                })
                .ToListAsync();


            var upcomingTreks = await _context.Availabilities
                        .Include(a => a.Trek)
                        .Where(a => a.StartDate >= DateTime.Now && a.StartDate <= DateTime.Now.AddMonths(1))
                        .OrderBy(a => a.StartDate)
                        .GroupBy(a => a.TrekId).CountAsync();


            var bookingsByRegion = await _context.Bookings
                .Include(b => b.Trek)
                .Where(b => b.PaymentSuccess == true)
                .GroupBy(b => b.Trek.Region)
                .Select(g => new { Region = g.Key, Count = g.Count() })

                .ToDictionaryAsync(g => g.Region, g => g.Count);

            var allTreks = await _context.Bookings
                .Include(b => b.Trek)
                .Where(b => b.PaymentSuccess == true)
                .GroupBy(b => b.Trek.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => new TrekPopularity
                {
                    TrekName = g.Key,
                    BookingCount = g.Count()
                })
                .ToListAsync();
            var topTreks = allTreks.Take(5).ToList();



            return new DashboardAnalyticsViewModel
            {
                TotalBookings = totalBookings,
                TotalUsers = totalUsers,
                CompletedTreks = completedTreks,
                OngoingTreks = ongoingTrekscount,
                TotalTreks=totaltreks,
                UserDetails = users,
                AllTreks = allTreks,
                TotalRevenue=totalrevenue,
                UpcomingTreks=upcomingTreks,
                Cancellations=totalCancels,
                BookingsByRegion = bookingsByRegion,
                TopTreks = topTreks,
                Treks=treks,
                OngoingTrekDetails=ongoingTreks
            };
        }
    }

}
