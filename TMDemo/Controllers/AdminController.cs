namespace TrekMasters.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IProfileService _profileService;

        public AdminController(IAdminService adminService, IProfileService profileService)
        {
            _adminService = adminService;
            _profileService = profileService;
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
            ModelState.Remove("Month");
            ModelState.Remove("EndDate");

            var trek = await _adminService.GetTrekByIdAsync(availability.TrekId);
            if (trek == null)
            {
                ModelState.AddModelError("", "Invalid Trek selected.");
                ViewBag.Treks = await _adminService.GetAllTreksAsync();
                return View(availability);
            }

            if (availability.StartDate == default)
            {
                ModelState.AddModelError("StartDate", "Start date is required.");
                ViewBag.Treks = await _adminService.GetAllTreksAsync();
                return View(availability);
            }

            availability.Month = availability.StartDate.ToString("MMMM");
            availability.EndDate = availability.StartDate.AddDays(trek.DurationDays - 1);
            var existingAvailability = await _adminService.CheckAvailabilityConflictAsync(availability.TrekId, availability.StartDate, availability.EndDate);
            if (existingAvailability)
            {
                TempData["Message"] = $"Can't add Availability for {trek.Name} at that date {availability.StartDate.ToShortDateString()} to {availability.EndDate.ToShortDateString()} because it is overlapping with exsisting Availabilities.";
                return RedirectToAction("Index","Admin");
           
            }
            if (ModelState.IsValid)
            {
               
                await _adminService.AddAvailabilityAsync(availability.TrekId, availability.StartDate, availability.EndDate,availability.Month,availability.MaxGroupSize);
                TempData["Message"] = "Availability added Successfully.";
                return RedirectToAction("Index", "Admin");
                
            }

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

            ViewBag.TrekName = trekName;
            ViewBag.TrekStartDate = trekStartDate.ToString("yyyy-MM-dd");
            return View(users);
        }
        public async Task<IActionResult> UserDetails(int bookingId)
        {
            if (bookingId == 0)
            {
                return BadRequest("Booking ID is required.");
            }

            var booking = await _adminService.GetBookingByIdAsync(bookingId);  
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            var participants = await _adminService.GetParticipantsForBookingAsync(bookingId);

            var user = await _profileService.GetUserAsync(booking.UserId);  
            if (user == null)
            {
                return NotFound("User not found.");
            }

            ViewBag.UserName = user.UserName;
            ViewBag.UserEmail = user.Email;
            ViewBag.BookingDate = booking.BookingDate.ToString("yyyy-MM-dd");

            var participantViewModels = participants.Select(p => new TrekParticipantViewModel
            {
                Name = p.Name,
                Email = p.Email,
                ContactNumber = p.ContactNumber
            }).ToList();

            return View(participantViewModels);
        }
        //[HttpPost]
        //public IActionResult Cancel(int availabilityId)
        //{
        //    try
        //    {
        //        bool isCancelled = _adminService.CancelTrek(availabilityId);

        //        if (isCancelled)
        //        {
        //            TempData["SuccessMessage"] = "Trek cancelled successfully, and refunds were processed.";
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "Failed to cancel the trek. Please try again.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = $"Error: {ex.Message}";
        //    }

        //    return RedirectToAction("Bookings", new { availabilityId });
        //}
        public async Task<IActionResult> CancelTrek(int availabilityId)
        {
            var model = await _adminService.GetTrekCancellationViewModelAsync(availabilityId);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ProcessTrekCancellation(TrekCancellationViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _adminService.ProcessTrekCancellationAsync(model);
                TempData["SuccessMessage"] = "Trek cancelled successfully, and refunds have been processed and users will be informed through Mail.";
                return RedirectToAction("Index", "Admin");
            }
            return View("CancelTrek", model);
        }

    }

}