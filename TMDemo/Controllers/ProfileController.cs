namespace TrekMasters.Controllers
{

    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _profileService.GetUserAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }

        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _profileService.GetUserAsync(userId);
            var emergencyContact = await _profileService.GetEmergencyContactAsync(userId);

            return View(new ProfileViewModel { UserDetail = user, EmergencyContact = emergencyContact ?? new EmergencyContact() });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _profileService.GetUserAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            user.FirstName = model.UserDetail.FirstName;
            user.LastName = model.UserDetail.LastName;
            user.PhoneNumber = model.UserDetail.PhoneNumber;
            user.DOB = model.UserDetail.DOB;
            user.Gender = model.UserDetail.Gender;
            user.Country = model.UserDetail.Country;
            user.City = model.UserDetail.City;
            user.State = model.UserDetail.State;

            var updateResult = await _profileService.UpdateUserAsync(user);
            if (!updateResult)
            {
                ModelState.AddModelError(string.Empty, "Error updating user details.");
                return View(model);
            }

            await _profileService.AddOrUpdateEmergencyContactAsync(userId, model.EmergencyContact);
            
            return RedirectToAction("Index");
        }

		public async Task<IActionResult> MyBookings()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var viewModel = await _profileService.GetBookingsByUserIdAsync(userId);
			return View(viewModel);
		}

	}


}