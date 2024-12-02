using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDemo.Data;
using TMDemo.Models;
using TMDemo.ViewModel;
namespace TMDemo.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult AddTrek()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTrek(AddTrekViewModel model)
        {
            if (ModelState.IsValid)
            {
                Trek trek = new Trek
                {
                    Name = model.Name,
                    Region = model.Region,
                    Description = model.Description,
                    DifficultyLevel = model.DifficultyLevel,
                    DurationDays = model.DurationDays,
                    HighAltitude = model.HighAltitude,
                    Price = model.Price,
                    Season = model.SelectedSeasons,
                    
                };

                if (model.TrekImgFile != null)
                {
                    using MemoryStream memoryStream = new MemoryStream();
                    await model.TrekImgFile.CopyToAsync(memoryStream);
                    trek.TrekImg = memoryStream.ToArray();
                }

                
                _context.Treks.Add(trek);
                await _context.SaveChangesAsync();

                return RedirectToAction("AddTrekPlan", new { trekId = trek.TrekId });
            }

            return View(model);
        }
        public IActionResult AddTrekPlan(int trekId)
        {
            Trek trek = _context.Treks.FirstOrDefault(t => t.TrekId == trekId);
            if (trek == null)
            {
                return NotFound("Trek not found.");
            }

            TrekPlanViewModel viewModel = new TrekPlanViewModel
            {
                TrekId = trekId,
                DurationDays = trek.DurationDays,
                Activities = new List<ActivityInputModel>()
            };
            for (int i = 1; i <= trek.DurationDays; i++)
            {
                viewModel.Activities.Add(new ActivityInputModel { Day = i });
            }

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTrekPlan(TrekPlanViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var activity in model.Activities)
                {
                    TrekPlan trekPlan = new TrekPlan
                    {
                        TrekId = model.TrekId,
                        Day = activity.Day,
                        ActivityDescription = activity.ActivityDescription
                    };

                    _context.TrekPlans.Add(trekPlan);
                }

                _context.SaveChanges(); 
                return RedirectToAction("Treks", "Admin");
            }

            return View(model); 
        }
        [HttpGet]
        public async Task<IActionResult> AddAvailability()
        {
            
            ViewBag.Treks = await _context.Treks
                .OrderBy(t => t.Name)
                .ToListAsync();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAvailability(Availability availability)

        {
            ModelState.Remove("Month");
            ModelState.Remove("EndDate");
            Trek trek = await _context.Treks.FindAsync(availability.TrekId);
            if (trek == null)
            {
                ModelState.AddModelError("", "Invalid Trek selected.");
                ViewBag.Treks = await _context.Treks.OrderBy(t => t.Name).ToListAsync();
                return View(availability);
            }
           
            availability.Month = availability.StartDate.ToString("MMMM");
            if (string.IsNullOrWhiteSpace(availability.Month))
            {
                ModelState.AddModelError("Month", "Month is required.");
            }
            availability.EndDate = availability.StartDate.AddDays(trek.DurationDays - 1);
            if (ModelState.IsValid)
            {
                _context.Availabilities.Add(availability);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","Admin");
            }

            
            ViewBag.Treks = await _context.Treks.OrderBy(t => t.Name).ToListAsync();
            return View(availability);
        }
        public async Task<IActionResult> Index()
        {
           
            List<Availability> availabilities = await _context.Availabilities
                .Include(a => a.Trek)
                .OrderBy(a => a.StartDate)
                .ToListAsync();

            return View(availabilities);
        }
        public ActionResult Treks()
        {
            List<Trek> treks = _context.Treks.ToList();
            return View(treks);
        }
        public ActionResult TrekBookings(int trekId)
        {
            var bookings = _context.Availabilities
                .Where(a => a.TrekId == trekId)
                .Select(a => new TrekAvailabilityViewModel
                {
                    AvailabilityId = a.AvailabilityId,
                    StartDate = a.StartDate,
                    MaxGroupSize = a.MaxGroupSize,
                    RemainingSlots = a.MaxGroupSize - (_context.Bookings
                        .Where(b => b.TrekId == trekId && b.TrekStartDate == a.StartDate)
                        .Sum(b => b.NumberOfPeople)-
                        _context.Bookings.Where(b=>b.TrekId==trekId && b.TrekStartDate== a.StartDate && (b.IsCancelled == true)).Sum(b => b.NumberOfPeople))
                })
                .ToList();

            string trekName = _context.Treks.FirstOrDefault(t => t.TrekId == trekId)?.Name;
            ViewBag.TrekName = trekName;

            return View(bookings);
        }
        public ActionResult Users(int availabilityId)
        {
            
            DateTime startDate = _context.Availabilities
                .Where(a => a.AvailabilityId == availabilityId)
                .Select(a => a.StartDate)
                .FirstOrDefault();

            if (startDate == default)
            {
                
                return NotFound("Availability not found.");
            }

            
            var users = _context.Bookings
                .Where(b => b.TrekStartDate == startDate && (b.IsCancelled == false || b.IsCancelled == null)) 
                .Select(b => new UserViewModel
                {
                    UserName = b.User.FirstName,
                    Email = b.User.Email,
                    NumberOfPeople = b.NumberOfPeople,
                    BookingDate = b.BookingDate
                })
                .ToList();

            Availability availability = _context.Availabilities
                .Include(a => a.Trek) 
                .FirstOrDefault(a => a.AvailabilityId == availabilityId);

            ViewBag.TrekName = availability?.Trek?.Name;
            ViewBag.StartDate = availability?.StartDate.ToString("yyyy-MM-dd");

            return View(users);
        }

    }
}
