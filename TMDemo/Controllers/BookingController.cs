using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDemo.Data;
using TMDemo.Models;
using TMDemo.Models.TMDemo.Models;

namespace TMDemo.Controllers
{
    [Authorize]
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

            var trek = _context.Treks.FirstOrDefault(t => t.TrekId == trekId);
            if (trek == null)
            {
                return NotFound();
            }
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            List<string> emails = new List<String>
            {
                userEmail
            };

            var viewModel = new AddUsersViewModel
            {
                TrekId = trekId,
                TrekName = trek.Name,
                StartDate = parsedDate,
                Emails = emails // Initialize the AddedUsers list
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
                    // Add an empty email field
                    model.Emails.Add(email);
                    return View(model);
                }
                else if (action == "Book")
                {

                    var trek = _context.Treks.FirstOrDefault(t => t.TrekId == model.TrekId);
                    if (trek == null)
                    {
                        return NotFound();
                    }

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieves logged-in user ID

                    var amount = trek.Price * (model.Emails.Count); // Calculate total amount
                    var tax = amount * 0.1M;
                    var totalAmount = amount + tax + 10;

                    var booking = new Booking
                    {
                        TrekId = model.TrekId,
                        UserId = userId,
                        BookingDate = DateTime.Now,
                        NumberOfPeople = model.Emails.Count,
                        TotalAmount = totalAmount,
                        TrekStartDate = model.StartDate,
                        CancellationDate = DateTime.MinValue // A default placeholder for non-cancelled bookings.

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
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
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
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
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
            // Retrieve the booking details from the database
            var booking = _context.Bookings
                .Include(b => b.Trek)  // Load related Trek details
                .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Calculate refund amount based on the business rules
            var startDate = booking.TrekStartDate;
            var cancelDate = DateTime.Now;
            var totalAmount = booking.TotalAmount;
            decimal refundAmount = 0;

            // Calculate days between booking date and cancellation date
            var daysDifference = (startDate - cancelDate).Days;

            if (daysDifference > 30)
            {
                refundAmount = totalAmount;
            }
            else if (daysDifference <= 30 && daysDifference > 20)
            {
                refundAmount = totalAmount * 0.9M; // 10% cutoff
            }
            else if (daysDifference <= 20 && daysDifference > 10)
            {
                refundAmount = totalAmount * 0.8M; // 20% cutoff
            }
            else if (daysDifference <= 10)
            {
                refundAmount = 0; // No refund
            }

            // Prepare the cancellation view model
            var model = new CancellationViewModel
            {
                BookingId = booking.BookingId,
                Booking = booking,
                RefundAmount = refundAmount // This will be displayed on the form
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ProcessCancellation(CancellationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CancelBooking", model); // Re-render the cancellation form if validation fails
            }

            // Retrieve the booking from the database
            var book = await _context.Bookings
                .Include(b => b.Trek) // Ensure the Trek is loaded
                .FirstOrDefaultAsync(b => b.BookingId == model.BookingId);

            if (book == null)
            {
                return NotFound("Booking not found.");
            }

            // Calculate refund amount again (although it was already done in the GET request)

            decimal refundAmount = model.RefundAmount;



            // Create the cancellation entry in the database
            book.IsCancelled = true;

            book.Reason = model.Reason;
            book.RefundAmount = refundAmount;
            book.CancellationDate = DateTime.Now;

            await _context.SaveChangesAsync();

            // Redirect to MyBookings page
            return RedirectToAction("MyBookings", "Profile");
        }
        [Authorize]
        [HttpGet]
        public IActionResult RescheduleBooking(int bookingId)
        {
            var booking = _context.Bookings
                .Include(b => b.Trek) // Load related trek details
                .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Fetch all available dates for the specific trek
            var availableDates = _context.Availabilities
                .Where(a => a.TrekId == booking.TrekId && a.StartDate > DateTime.Now)
                .Select(a => a.StartDate)
                .Distinct() // Ensure unique dates
                .OrderBy(d => d) // Sort dates in ascending order
                .ToList();

            // Handle case where no available dates exist
            if (availableDates == null || !availableDates.Any())
            {
                ModelState.AddModelError("", "No available dates for this trek.");
                return View("Error");
            }

            var model = new RescheduleViewModel
            {
                BookingId = booking.BookingId,
                OldStartDate = booking.TrekStartDate,
                AvailableDates = availableDates, 
                ExtraAmount = 0 
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

            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == model.BookingId);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            
            var daysDifference = (model.OldStartDate - DateTime.Now).Days;
            decimal extraAmount = 0;
            if (daysDifference <= 25)
            {
                extraAmount = booking.TotalAmount * 0.05M; 
            }
            booking.ExtraAmount = extraAmount;
            booking.RescheduleReason = model.Reason;
            booking.TrekStartDate = model.NewStartDate;
            booking.TotalAmount += extraAmount;
            if (model.OldStartDate != model.NewStartDate)
            {
				_context.SaveChanges();

				return View("RescheduleSuccess");
			}
            return View(model);
        }




    }
}
