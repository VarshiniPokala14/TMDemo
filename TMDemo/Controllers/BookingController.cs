namespace TMDemo.Controllers
{
    [Authorize(Roles ="User")]
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<UserDetail> _userManager;
        public BookingController(AppDbContext context, UserManager<UserDetail> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        public IActionResult AddUsers(int trekId, string startDate)
        {

            Console.WriteLine($"trekId: {trekId}, startDate: {startDate}");

            DateTime parsedDate;
            if (!DateTime.TryParse(startDate, out parsedDate))
            {
                return BadRequest("Invalid date format.");
            }

            Trek trek = _context.Treks.FirstOrDefault(t => t.TrekId == trekId);
            if (trek == null)
            {
                return NotFound();
            }
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            List<string> emails = new List<String>
            {
                userEmail
            };

            AddUsersViewModel viewModel = new AddUsersViewModel
            {
                TrekId = trekId,
                TrekName = trek.Name,
                StartDate = parsedDate,
                Emails = emails 
            };

            return View(viewModel);
        }


        [Authorize]
        [HttpPost]
        public IActionResult AddUsers(AddUsersViewModel model, string action, string? email)
        {
            if (ModelState.IsValid)
            {
                if (action == "AddMember")
                {
                    
                    model.Emails.Add(email);
                    return View(model);
                }
                else if (action == "Book")
                {

                    Trek trek = _context.Treks.FirstOrDefault(t => t.TrekId == model.TrekId);
                    if (trek == null)
                    {
                        return NotFound();
                    }

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

                    decimal amount = trek.Price * (model.Emails.Count); 
                    decimal tax = amount * 0.05M;
                    decimal totalAmount = amount + tax + 10;

                    Booking booking = new Booking
                    {
                        TrekId = model.TrekId,
                        UserId = userId,
                        BookingDate = DateTime.Now,
                        NumberOfPeople = model.Emails.Count,
                        TotalAmount = totalAmount,
                        TrekStartDate = model.StartDate,
                        CancellationDate = DateTime.MinValue 

                    };

                    _context.Bookings.Add(booking);

                    Console.WriteLine($"BookingDate: {booking.BookingDate}");

                    _context.SaveChanges();


                    return View("PaymentPage", booking);
                }
            }
            return Ok(ModelState);
        }
        [Authorize]
        public IActionResult PaymentPage(int bookingId)
        {
            Booking booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (booking == null)
            {
                return NotFound();
            }

            return View("PaymentPage", booking);
        }
        [Authorize]
        [HttpPost]
        public IActionResult ProcessPayment(int bookingId, string paymentMethod)
        {
            Booking booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (booking == null)
            {
                return NotFound();
            }

            booking.PaymentSuccess = true;
            _context.SaveChanges();

            

            return View("BookingSuccess");
        }
        [HttpGet]
        public IActionResult CancelBooking(int bookingId)
        {
            
            Booking booking = _context.Bookings
                .Include(b => b.Trek)  
                .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            
            DateTime startDate = booking.TrekStartDate;
            DateTime cancelDate = DateTime.Now;
            decimal totalAmount = booking.TotalAmount;
            decimal refundAmount = 0;

            
            var daysDifference = (startDate - cancelDate).Days;

            if (daysDifference > 30)
            {
                refundAmount = totalAmount;
            }
            else if (daysDifference <= 30 && daysDifference > 20)
            {
                refundAmount = totalAmount * 0.9M; 
            }
            else if (daysDifference <= 20 && daysDifference > 10)
            {
                refundAmount = totalAmount * 0.8M; 
            }
            else if (daysDifference <= 10)
            {
                refundAmount = 0; 
            }

           
            CancellationViewModel model = new CancellationViewModel
            {
                BookingId = booking.BookingId,
                Booking = booking,
                RefundAmount = refundAmount 
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ProcessCancellation(CancellationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CancelBooking", model); 
            }

            
            Booking book = await _context.Bookings
                .Include(b => b.Trek) 
                .FirstOrDefaultAsync(b => b.BookingId == model.BookingId);

            if (book == null)
            {
                return NotFound("Booking not found.");
            }
            decimal refundAmount = model.RefundAmount;
            book.IsCancelled = true;

            book.Reason = model.Reason;
            book.RefundAmount = refundAmount;
            book.CancellationDate = DateTime.Now;

            await _context.SaveChangesAsync();

            
            return RedirectToAction("MyBookings", "Profile");
        }
        [Authorize]
        [HttpGet]
        public IActionResult RescheduleBooking(int bookingId)
        {
            Booking booking = _context.Bookings
                .Include(b => b.Trek) 
                .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            
            var availableDates = _context.Availabilities
                .Where(a => a.TrekId == booking.TrekId && a.StartDate > DateTime.Now)
                .Select(a => a.StartDate)
                .Distinct() 
                .OrderBy(d => d) 
                .ToList();

            
            if (availableDates == null || !availableDates.Any())
            {
                ModelState.AddModelError("", "No available dates for this trek.");
                return View("Error");
            }
            var daysDifference = (booking.TrekStartDate - DateTime.Now).Days;
            decimal extraAmount = 0;
            if (daysDifference <= 25)
            {
                extraAmount = booking.TotalAmount * 0.05M; 
            }
            RescheduleViewModel model = new RescheduleViewModel
            {
                BookingId = booking.BookingId,
                OldStartDate = booking.TrekStartDate,
                AvailableDates = availableDates, 
                ExtraAmount = extraAmount
            };

            return View(model);
        }



        [Authorize]
        [HttpPost]
        public IActionResult ProcessReschedule(RescheduleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("RescheduleBooking", model);
            }

            Booking booking = _context.Bookings.FirstOrDefault(b => b.BookingId == model.BookingId);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            
            //var daysDifference = (model.OldStartDate - DateTime.Now).Days;
            //decimal extraAmount = 0;
            //if (daysDifference <= 25)
            //{
            //    extraAmount = booking.TotalAmount * 0.05M; 
            //}
            booking.ExtraAmount = model.ExtraAmount;
            booking.RescheduleReason = model.Reason;
            booking.TrekStartDate = model.NewStartDate;
            booking.TotalAmount += model.ExtraAmount;
            if (model.OldStartDate != model.NewStartDate)
            {
				_context.SaveChanges();

				return View("RescheduleSuccess");
			}
            return View("RescheduleBooking",model);
        }
    }
}
