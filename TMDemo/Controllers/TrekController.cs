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
            var treks = _context.Treks.ToList();
            
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
        public IActionResult ViewDates(int trekId)
        {
            
            var trek = _context.Treks
                            .Include(t => t.Availabilities)
                            .FirstOrDefault(t => t.TrekId == trekId);
            
            if (trek == null)
            {
                return NotFound();
            }

           
            var availabilityDates = trek.Availabilities
                .GroupBy(a => a.StartDate.ToString("MMMM yyyy")) 
                .Select(group => new MonthAvailability
                {
                    Month = group.Key, 
                    Dates = group.Select(a => new DateRange
                    {
                        StartDate = a.StartDate,
                        EndDate = a.EndDate
                    }).ToList()
                })
                .ToList();

            
            var viewModel = new TrekDetailsViewModel
            {
                Trek = trek,
                AvailabilityDates = availabilityDates
            };

          
            return View(viewModel);
        }
        public IActionResult Details(int trekId)
        {
            var trek = _context.Treks
                .Include(t => t.Availabilities)
                .Include(t => t.TrekReviews) 
                .ThenInclude(r => r.User) 
                .FirstOrDefault(t => t.TrekId == trekId);

            if (trek == null)
            {
                return NotFound();
            }


             var availabilityDates = trek.Availabilities
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

            var viewModel = new TrekDetailsViewModel
            {
                Trek = trek,
                AvailabilityDates = availabilityDates,
                Reviews = trek.TrekReviews.OrderByDescending(r => r.CreatedAt).ToList() 
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

    [HttpPost]
        public async Task<IActionResult> Add(TrekReview review)
        {
            if (ModelState.IsValid)
            {
                review.CreatedAt = DateTime.Now;
                review.UserId = _userManager.GetUserId(User);
                _context.TrekReviews.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Trek", new { trekId = review.TrekId });
            }

            return View(review);
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
