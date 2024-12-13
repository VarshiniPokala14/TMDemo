
using TrekMasters.Service;

namespace TrekMasters.Controllers
{
    [Authorize(Roles ="User")]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly UserManager<UserDetail> _userManager;
        private readonly IProfileService _profileService;
        private readonly IValidationService _validationService;
        public BookingController(IBookingService bookingService, UserManager<UserDetail> userManager, IProfileService profileService,
        IValidationService validationService)
        {
            _bookingService = bookingService;
            _userManager = userManager;
            _profileService = profileService;
            _validationService = validationService;
        }

        
        public async Task<IActionResult> AddUsers(int trekId, string startDate)
        {
            string userEmail = User.FindFirstValue(ClaimTypes.Email);
            var viewModel = _bookingService.GetAddUsersViewModel(trekId, startDate, userEmail);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddUsers(AddUsersViewModel model, string action, string? email)
            {
            // Ensure Participants is initialized
            if (model.Participants == null && action == "AddMember")
            {
                ModelState.Remove("Participants");
                model.Participants = new List<ParticipantViewModel>();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            switch (action)
            {
                case "AddMember":
                    return await HandleAddMember(model, email);

                case var removeAction when removeAction.StartsWith("RemoveMember-"):
                    return await HandleRemoveMember(model, removeAction);

                case "Book":
                    return await HandleBookingAsync(model);

                default:
                    ModelState.AddModelError("", "Invalid action.");
                    return View(model);
            }
        }

        private async Task<IActionResult> HandleAddMember(AddUsersViewModel model, string? email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email is required.");
                return View(model);
            }

            if (_validationService.IsEmailAlreadyAdded(model.Participants, email))
            {
                ModelState.AddModelError("", "This email is already added.");
            }
            else
            {
                model.Participants.Add(new ParticipantViewModel { Email = email });
            }

            ModelState.Clear();
            return View(model);
        }

        private async Task<IActionResult> HandleRemoveMember(AddUsersViewModel model, string action)
        {
            if (int.TryParse(action.Replace("RemoveMember-", ""), out int indexToRemove))
            {
                if (_validationService.IsParticipantIndexValid(indexToRemove, model.Participants.Count))
                {
                    model.Participants.RemoveAt(indexToRemove);
                    ModelState.Clear();
                }
                else
                {
                    ModelState.AddModelError("", "Invalid participant index.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid index format.");
            }

            return View(model);
        }

        private async Task<IActionResult> HandleBookingAsync(AddUsersViewModel model)
        {
            if (!model.Participants.Any())
            {
                ModelState.AddModelError("", "Please add at least one participant before proceeding.");
                return View(model);
            }

            try
            {
                string userId = _profileService.GetCurrentUserId();
                //Booking booking = await _bookingService.CreateBookingAsync(model, userId);
                var (booking, conflictWarnings) = await _bookingService.CreateBookingAsync(model, userId);

                foreach (var participant in model.Participants)
                {
                    _bookingService.AddParticipant(booking.BookingId, participant);
                }
                if (conflictWarnings.Any())
                {
                    TempData["ConflictWarnings"] = conflictWarnings;
                }
                return View("PaymentPage", booking);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int bookingId, string paymentMethod)
        {
 
            var paymentProcessed = await _bookingService.ProcessPayment(bookingId, paymentMethod);

            if (!paymentProcessed)
            {
                return NotFound(); 
            }
            return View("BookingSuccess");
        }
        
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var model = _bookingService.GetCancellationViewModel(bookingId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCancellation(CancellationViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _bookingService.ProcessCancellation(model);
                return RedirectToAction("MyBookings", "Profile");
            }
            return View("CancelBooking", model);
        }

       
        [HttpGet]
        public async Task<IActionResult> RescheduleBooking(int bookingId)
        {
            var model =  _bookingService.GetRescheduleViewModel(bookingId);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ProcessReschedule(RescheduleViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                _bookingService.ProcessReschedule(model);
                if (model.OldStartDate != model.NewStartDate)
                {
                    return View("RescheduleSuccess");
                }
                return RedirectToAction("MyBookings", "Profile");
            }
            return View("RescheduleBooking", model);
        }
     
    }

}











