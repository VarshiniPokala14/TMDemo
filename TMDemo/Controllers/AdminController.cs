
using TMDemo.Models;
using TMDemo.Service;

namespace TMDemo.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
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
                int trekId = await _adminService.AddTrekAsync(model);

                return RedirectToAction("AddTrekPlan", new { trekId });
            }

            return View(model);
        }

        public async Task<IActionResult> AddTrekPlan(int trekId)
        {
            Trek trek = await _adminService.GetTrekByIdAsync(trekId);
            if (trek == null)
            {
                return NotFound("Trek not found.");
            }

            var viewModel = new TrekPlanViewModel
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
        public async Task<IActionResult> AddTrekPlan(TrekPlanViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _adminService.AddTrekPlanAsync(model);
                return RedirectToAction("Treks", "Admin");
            }

            return View(model);
        }

        // Add Availability (GET)
        [HttpGet]
        public async Task<IActionResult> AddAvailability()
        {
            ViewBag.Treks = await _adminService.GetAllTreksAsync();
            return View();
        }

        // Add Availability (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAvailability(Availability availability)
        {
            // Remove unnecessary validations if already handled
            ModelState.Remove("Month");
            ModelState.Remove("EndDate");

            // Validate Trek existence
            var trek = await _adminService.GetTrekByIdAsync(availability.TrekId);
            if (trek == null)
            {
                ModelState.AddModelError("", "Invalid Trek selected.");
                ViewBag.Treks = await _adminService.GetAllTreksAsync();
                return View(availability);
            }

            // Validate StartDate
            if (availability.StartDate == default)
            {
                ModelState.AddModelError("StartDate", "Start date is required.");
                ViewBag.Treks = await _adminService.GetAllTreksAsync();
                return View(availability);
            }

            // Set derived properties
            availability.Month = availability.StartDate.ToString("MMMM");
            availability.EndDate = availability.StartDate.AddDays(trek.DurationDays - 1);

            // Save availability if model is valid
            if (ModelState.IsValid)
            {
                await _adminService.AddAvailabilityAsync(availability);
                return RedirectToAction("Index", "Admin");
            }

            // Reload Treks for the View
            ViewBag.Treks = await _adminService.GetAllTreksAsync();
            return View(availability);
        }


        public async Task<IActionResult> Index()
        {
            var availabilities = await _adminService.GetAllAvailabilitiesAsync();
            return View(availabilities);
        }

        public async Task<IActionResult> Treks()
        {
            var treks = await _adminService.GetAllTreksAsync();
            return View(treks);
        }

        public async Task<IActionResult> TrekBookings(int trekId)
        {
            var bookings = await _adminService.GetTrekBookingsAsync(trekId);
            Trek trek = await _adminService.GetTrekByIdAsync(trekId);
            string trekName = trek?.Name;

            ViewBag.TrekName = trekName;
            return View(bookings);
        }

        public async Task<IActionResult> Users(int availabilityId)
        {
            var users = await _adminService.GetUsersForAvailabilityAsync(availabilityId);
            var availability =  _adminService.GetAvailabilityById(availabilityId);
            string trekName = availability.Trek.Name;
            DateTime trekStartDate = availability.StartDate;

            // Pass the data to the View via ViewBag
            ViewBag.TrekName = trekName;
            ViewBag.TrekStartDate = trekStartDate.ToString("yyyy-MM-dd");
            return View(users);
        }
    }

}