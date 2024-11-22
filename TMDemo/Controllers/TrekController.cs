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
                .Include(t => t.Availabilities) // Ensure Availabilities is included
                .Where(t => t.Availabilities.Any(a => a.StartDate >= DateTime.Now && a.StartDate <= threeMonthsLater))
                .ToListAsync();

            return View(treks);
        }
        //public IActionResult ViewDates(int trekId)
        //{
        //    // Fetch the trek and its availability
        //    var trek = _context.Treks
        //        .Include(t => t.Availabilities) // Load related availability data
        //        .FirstOrDefault(t => t.TrekId == trekId);

        //    if (trek == null)
        //    {
        //        return NotFound("Trek not found.");
        //    }
        //    var groupedAvailability = trek.Availabilities
        //        .OrderBy(a => a.StartDate) // Sort by start date
        //        .GroupBy(a => new { a.StartDate.Year, a.StartDate.Month }) // Group by year and month
        //        .Select(g => new
        //        {
        //            Year = g.Key.Year,
        //            Month = g.Key.Month,
        //            Dates = g.Select(a => new
        //            {
        //                StartDate = a.StartDate,
        //                EndDate = a.EndDate,
        //                MaxGroupSize = a.MaxGroupSize
        //            }).ToList()
        //        })
        //    .ToList();

        //    ViewBag.GroupedAvailability = groupedAvailability;

        //    return View(trek);
        //}
        //public IActionResult ViewDates(int trekId)
        //{
        //    var trek = _context.Treks
        //        .Include(t => t.Availabilities)
        //        .FirstOrDefault(t => t.TrekId == trekId);

        //    if (trek == null)
        //    {
        //        return NotFound();
        //    }

        //    // Group availability data by Month
        //    var groupedAvailability = trek.Availabilities
        //        .OrderBy(a => a.StartDate)
        //        .GroupBy(a => a.Month)
        //        .ToDictionary(g => g.Key, g => g.ToList());

        //    var viewModel = new TrekDatesViewModel
        //    {
        //        TrekName = trek.Name,
        //        GroupedAvailabilitiesByMonth = groupedAvailability
        //    };

        //    return View(viewModel);
        //}

        public IActionResult ViewDates(int trekId)
        {
            // Retrieve the trek and its availabilities
            var trek = _context.Treks
                            .Include(t => t.Availabilities)
                            .FirstOrDefault(t => t.TrekId == trekId);
            
            if (trek == null)
            {
                return NotFound();
            }

            // Group availability dates by month and map to MonthAvailability model
            var availabilityDates = trek.Availabilities
                .GroupBy(a => a.StartDate.ToString("MMMM yyyy")) // Group by Month and Year
                .Select(group => new MonthAvailability
                {
                    Month = group.Key, // e.g., "March 2025"
                    Dates = group.Select(a => new DateRange
                    {
                        StartDate = a.StartDate,
                        EndDate = a.EndDate
                    }).ToList()
                })
                .ToList();

            // Populate the view model
            var viewModel = new TrekDetailsViewModel
            {
                Trek = trek,
                AvailabilityDates = availabilityDates
            };

            // Pass the view model to the view
            return View(viewModel);
        }
        public IActionResult Details(int trekId)
        {
            // Fetch the trek details including availability
            var trek = _context.Treks
                .Include(t => t.Availabilities)
                .FirstOrDefault(t => t.TrekId == trekId);

            if (trek == null)
            {
                return NotFound();
            }
            
            // Group availability dates by month and map to MonthAvailability model
            var availabilityDates = trek.Availabilities
                .GroupBy(a => a.StartDate.ToString("MMMM yyyy")) // Group by Month and Year
                .Select(group => new MonthAvailability
                {
                    Month = group.Key, // e.g., "March 2025"
                    Dates = group.Select(a => new DateRange
                    {
                        StartDate = a.StartDate,
                        EndDate = a.EndDate
                    }).ToList()
                })
                .ToList();

            // Populate the view model
            var viewModel = new TrekDetailsViewModel
            {
                Trek = trek,
                

                AvailabilityDates = availabilityDates
            };

            // Pass the view model to the view
            return View(viewModel);
        }





    }

}
