using TrekMasters.Models;

namespace TrekMasters.Service
{
    public class AnalyticsService
    {
        private readonly AppDbContext _context;

        public AnalyticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardAnalyticsViewModel> GetDashboardDataAsync()
        {
            var bookings = await _context.Bookings.Include(t => t.Trek).Where(b => b.PaymentSuccess == true).ToListAsync();
            var totalBookings = bookings.Count();
            var totalUsers = await _context.Users.CountAsync();
            var completedTreks = bookings.Where(b => (b.TrekStartDate.AddDays(b.Trek.DurationDays - 1) <= DateTime.Now)).GroupBy(b => new { b.TrekId, b.TrekStartDate }).Count();
            var ongoingTreks = bookings
                    .Where(b => (b.TrekStartDate <= DateTime.Now)
                    && b.TrekStartDate.AddDays(b.Trek.DurationDays - 1) >= DateTime.Now).Count();
            var totalCancels=bookings.Where(b=>b.IsCancelled==true).Count();
            var totaltreks = await _context.Treks.CountAsync();
            var finaltotal=bookings.Sum(b=>b.TotalAmount);
            var extraAmount = bookings.Sum(b => b.ExtraAmount ?? 0);
            var refundAmount=bookings.Sum(b=>b.RefundAmount ?? 0);  
            var totalrevenue=finaltotal+extraAmount-refundAmount;
            var upcomingTreks = await _context.Availabilities
                        .Include(a => a.Trek)
                        .Where(a => a.StartDate >= DateTime.Now && a.StartDate <= DateTime.Now.AddMonths(1))
                        .OrderBy(a => a.StartDate)
                        .GroupBy(a=>a.TrekId).CountAsync();


            var bookingsByRegion = await _context.Bookings
                .Include(b => b.Trek)
                .Where(b=>b.PaymentSuccess==true)
                .GroupBy(b => b.Trek.Region)
                .Select(g => new { Region = g.Key, Count = g.Count() })
                
                .ToDictionaryAsync(g => g.Region, g => g.Count);

            var topTreks = await _context.Bookings
                .Include(b => b.Trek)
                .Where(b=>b.PaymentSuccess==true)
                .GroupBy(b => b.Trek.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => new TrekPopularity
                {
                    TrekName = g.Key,
                    BookingCount = g.Count()
                })
                .Take(5)
                .ToListAsync();

            return new DashboardAnalyticsViewModel
            {
                TotalBookings = totalBookings,
                TotalUsers = totalUsers,
                CompletedTreks = completedTreks,
                OngoingTreks = ongoingTreks,
                TotalTreks=totaltreks,
                TotalRevenue=totalrevenue,
                UpcomingTreks=upcomingTreks,
                Cancellations=totalCancels,
                BookingsByRegion = bookingsByRegion,
                TopTreks = topTreks
            };
        }
    }

}
