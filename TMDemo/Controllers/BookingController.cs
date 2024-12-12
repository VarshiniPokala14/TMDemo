
namespace TrekMasters.Controllers
{
    [Authorize(Roles ="User")]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly UserManager<UserDetail> _userManager;

        public BookingController(IBookingService bookingService, UserManager<UserDetail> userManager)
        {
            _bookingService = bookingService;
            _userManager = userManager;
        }

        
        public IActionResult AddUsers(int trekId, string startDate)
        {
            string userEmail = User.FindFirstValue(ClaimTypes.Email);
            var viewModel = _bookingService.GetAddUsersViewModel(trekId, startDate, userEmail);

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> AddUsers(AddUsersViewModel model, string action, string? email)
        {
            if (model.Participants == null && action == "AddMember")
            {
                ModelState.Remove("Participants");
                model.Participants = new List<ParticipantViewModel>();
            }

            if (ModelState.IsValid)
            {

                if (action == "AddMember" && !string.IsNullOrEmpty(email))
                {
                    if (!model.Participants.Any(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                    {
                        model.Participants.Add(new ParticipantViewModel { Email = email });
                    }
                    else
                    {
                        ModelState.AddModelError("", "This email is already added.");
                    }

                    ModelState.Clear();
                    return View(model);
                }
                else if (action.StartsWith("RemoveMember-"))
                {
                    if (int.TryParse(action.Replace("RemoveMember-", ""), out int indexToRemove))
                    {
                        if (indexToRemove >= 0 && indexToRemove < model.Participants.Count)
                        {
                            model.Participants.RemoveAt(indexToRemove);
                            ModelState.Clear();
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid participant index.");
                        }
                    }

                    return View(model);
                }
                else if (action == "Book")
                {
                    if (!model.Participants.Any())
                    {
                        ModelState.AddModelError("", "Please add at least one participant before proceeding.");
                        return View(model);
                    }

                    try
                    {
                        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                        var booking = await _bookingService.CreateBookingAsync(model, userId);

                        foreach (var participant in model.Participants)
                        {
                            _bookingService.AddParticipant(booking.BookingId, participant);
                        }

                        return View("PaymentPage", booking);
                    }
                    catch (InvalidOperationException ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                    }
                }
            }
            return View(model);
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
        
        public IActionResult CancelBooking(int bookingId)
        {
            var model = _bookingService.GetCancellationViewModel(bookingId);
            return View(model);
        }

        [HttpPost]
        public IActionResult ProcessCancellation(CancellationViewModel model)
        {
            if (ModelState.IsValid)
            {
                _bookingService.ProcessCancellation(model);
                return RedirectToAction("MyBookings", "Profile");
            }
            return View("CancelBooking", model);
        }

       
        [HttpGet]
        public IActionResult RescheduleBooking(int bookingId)
        {
            var model = _bookingService.GetRescheduleViewModel(bookingId);
            return View(model);
        }
        [HttpPost]
        public IActionResult ProcessReschedule(RescheduleViewModel model)
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