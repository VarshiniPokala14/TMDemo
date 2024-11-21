using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDemo.Data;
using TMDemo.Models;

namespace TMDemo.Controllers
{
    public class TrekController : Controller
    {
        private readonly AppDbContext _context;
        public TrekController(AppDbContext context)
        {
            _context = context;
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
                               .Where(t => t.TrekId == trekId)
                               .Include(t => t.Availabilities)
                               .FirstOrDefault(); // Use FirstOrDefault instead of ToList to get a single trek

            // If no trek is found, return a NotFound result
            if (trek == null)
            {
                return NotFound();
            }

            // Create a new variable to store the availability dates and months
            var availabilityDates = trek.Availabilities
                .GroupBy(a => a.StartDate.ToString("MMMM yyyy")) // Group by month and year (e.g., "July 2025")
                .Select(group => new AvailabilityDate
                {
                    Month = group.Key, // Group key is the "Month Year" format
                    Dates = group.OrderBy(a => a.StartDate) // Order dates in ascending order
                                 .Select(date => new DateRange
                                 {
                                     StartDate = date.StartDate,
                                     EndDate = date.EndDate,
                                     
                                 }).ToList()
                }).ToList();

            // Pass both the trek and the availabilityDates to the view
            var model = new TrekViewModel
            {
                Trek = trek,
                AvailabilityDates = availabilityDates
            };

            return View(model);
        }
        public IActionResult Details(int trekId)
        {
            var trek = _context.Treks
                               .Where(t => t.TrekId == trekId)
                               .Include(t => t.Availabilities)
                               .FirstOrDefault();
            return View(trek);
        }


    }

}
