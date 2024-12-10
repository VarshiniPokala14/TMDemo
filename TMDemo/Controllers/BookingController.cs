using TMDemo.Service;

namespace TMDemo.Controllers
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


        //[HttpPost]
        //public IActionResult AddUsers(AddUsersViewModel model, string action, string? email)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (action == "AddMember")
        //        {
        //            _bookingService.AddMember(model, email);
        //            return View(model);
        //        }
        //        else if (action.StartsWith("RemoveMember-"))
        //        {
        //            int indexToRemove;
        //            if (int.TryParse(action.Replace("RemoveMember-", ""), out indexToRemove))
        //            {
        //                _bookingService.RemoveMember(model, indexToRemove);
        //            }
        //            return View(model);
        //        }
        //        else if (action == "Book")
        //        {
        //            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //            var booking = _bookingService.CreateBooking(model, userId);

        //            return View("PaymentPage", booking);
        //        }
        //    }
        //    return Ok(ModelState);
        //}
        [HttpPost]
        public async Task<IActionResult> AddUsers(AddUsersViewModel model, string action, string? email)
        {
            if (ModelState.IsValid)
            {
                if (action == "AddMember")
                {
                    _bookingService.AddMember(model, email);
                    return View(model);
                }
                else if (action.StartsWith("RemoveMember-"))
                {
                    int indexToRemove;
                    if (int.TryParse(action.Replace("RemoveMember-", ""), out indexToRemove))
                    {
                        _bookingService.RemoveMember(model, indexToRemove);
                    }
                    return View(model);
                }
                else if (action == "Book")
                {
                    try
                    {
                        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        var booking = await _bookingService.CreateBookingAsync(model, userId);
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