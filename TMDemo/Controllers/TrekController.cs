using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDemo.Data;
using TMDemo.Models;


namespace TMDemo.Controllers
{
    public class TrekController : Controller
    {
        private readonly UserManager<UserDetail> _userManager;
        private readonly AppDbContext _context;
        public TrekController(AppDbContext context, UserManager<UserDetail> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult AllTreks()
        {
            var treks = _context.Treks
                .Include(t=>t.Availabilities)
                .ToList();
            
            return View(treks);
        }
        public IActionResult GetImage(int trekId)
        {
            var trek = _context.Treks.FirstOrDefault(t => t.TrekId == trekId);
            if (trek.TrekImg != null)
            {
                return File(trek.TrekImg, "image/jpg");
            }
            return NotFound();
        }
        public async Task<IActionResult> UpcomingTreks()
        {
            
            var today = DateTime.Today;
            var threeMonthsLater = today.AddMonths(3);            
            var treks = await _context.Treks
                .Include(t => t.Availabilities) 
                .Where(t => t.Availabilities.Any(a => a.StartDate >= DateTime.Now && a.StartDate <= threeMonthsLater))
                .ToListAsync();

            return View(treks);
        }
        public IActionResult Details(int trekId)
        {
            CleanupPastAvailabilities();
            var trek = _context.Treks
                .Include(t => t.Availabilities)
                .Include(t => t.TrekReviews)
                .Include(t => t.Bookings)
                .ThenInclude(r => r.User)
                .Include(t => t.TrekPlans)
                .FirstOrDefault(t => t.TrekId == trekId);

            if (trek == null)
            {
                return NotFound();
            }

            
            var currentDate = DateTime.Now;

            var userBooking = trek.Bookings.FirstOrDefault(b => b.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) );

            var trekEndDate = userBooking != null
                ? userBooking.TrekStartDate.AddDays(trek.DurationDays)
                : DateTime.MinValue;

            bool isTrekCompleted = trekEndDate <= currentDate;

            bool hasReviewed = trek.TrekReviews.Any(r => r.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));

            var availabilityDates = trek.Availabilities
                .OrderBy(date => date.StartDate.Year)
                .ThenBy(date => date.StartDate.Month)
                .GroupBy(a => a.StartDate.ToString("MMMM yyyy"))
                .Select(group => new MonthAvailability
                {
                    Month = group.Key,
                    Dates = group.Select(a => new DateRange
                    {
                        StartDate = a.StartDate,
                        EndDate = a.EndDate,
                        RemainingSlots = GetRemainingSlots(trek.TrekId, a.StartDate)
                    }).ToList()
                })
                .ToList();

            var trekplan = trek.TrekPlans.OrderBy(tp => tp.Day).Select(tp => tp.ActivityDescription).ToList();
            var viewModel = new TrekDetailsViewModel
            {
                Trek = trek,
                AvailabilityDates = availabilityDates,
                Reviews = trek.TrekReviews.OrderByDescending(r => r.CreatedAt).ToList(),
                TrekPlan = trekplan,
                Bookings = trek.Bookings.ToList(),
                IsTrekCompleted = isTrekCompleted,
                HasReviewed = hasReviewed
            };

            return View(viewModel);
        }

        private int GetRemainingSlots(int trekId, DateTime startDate)
        {
        
            int totalSlots = _context.Availabilities
            .Where(a => a.TrekId == trekId && a.StartDate == startDate)
            .Select(a => a.MaxGroupSize)
            .FirstOrDefault();

            int totalbookedSlots = _context.Bookings
            .Where(b => b.TrekId == trekId && b.TrekStartDate == startDate)
            .Sum(b => b.NumberOfPeople);

            int totalcancelledslots = _context.Bookings
                .Where(b => b.TrekId == trekId && b.TrekStartDate == startDate && (b.IsCancelled==true))
                .Sum(b => b.NumberOfPeople);
            return totalSlots - (totalbookedSlots - totalcancelledslots);
        }
        private void CleanupPastAvailabilities()
        {
            var pastAvailabilities = _context.Availabilities
                .Where(a => a.StartDate < DateTime.Now)
                .ToList();

            if (pastAvailabilities.Any())
            {
                _context.Availabilities.RemoveRange(pastAvailabilities);
                _context.SaveChanges();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int trekId, string reviewText)
        {
            var userId = _userManager.GetUserId(User);

            var booking = await _context.Bookings
                .Include(t=>t.Trek)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.TrekId == trekId && (b.IsCancelled==false||b.IsCancelled==null));

            if (booking == null)
            {
                return Ok("You must book the trek first." );

            }

            var trekEndDate = booking.TrekStartDate.AddDays(booking.Trek.DurationDays);

            if (trekEndDate > DateTime.Now)
            {
                return Ok("You can only leave a review after completing the trek.");
            }

            var reviewExists = await _context.TrekReviews
                .AnyAsync(r => r.UserId == userId && r.TrekId == trekId);

            if (reviewExists)
            {
                return Ok("You have already reviewed this trek.");
                
            }

            var review = new TrekReview
            {
                UserId = userId,
                TrekId = trekId,
                Comment = reviewText,
                CreatedAt = DateTime.Now
            };

            _context.TrekReviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { trekId });
        }


        public async Task<IActionResult> Index(string searchString)
        {
            if (_context.Treks == null)
            {
                return Problem("No related searches are available.");
            }

            var treks = from m in _context.Treks
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                treks = treks.Where(s => s.Name!.ToUpper().Contains(searchString.ToUpper()));
            }
            
            return View(await treks.ToListAsync());
        }
    }
}
