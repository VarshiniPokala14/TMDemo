using System.Reflection.Metadata.Ecma335;
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
        public BookingController(AppDbContext context,UserManager<UserDetail> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        public IActionResult AddUsers(int trekId, string startDate)
        {
            // Log the parameters for debugging
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

            var viewModel = new AddUsersViewModel
            {
                TrekId = trekId,
                TrekName = trek.Name,
                StartDate = parsedDate,
                AddedUsers = new List<AddedUser>() // Initialize the AddedUsers list
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult FinalBooking(AddUsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                var trek = _context.Treks.FirstOrDefault(t => t.TrekId == model.TrekId);
                if (trek == null)
                {
                    return NotFound();
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieves logged-in user ID

                var totalAmount = trek.Price * model.AddedUsers.Count; // Calculate total amount

                var booking = new Booking
                {
                    TrekId = model.TrekId,
                    UserId = userId, // Use the logged-in user's ID
                    BookingDate = DateTime.Now,
                    NumberOfPeople = model.AddedUsers.Count,
                    TotalAmount = totalAmount,
                    TrekStartDate = model.StartDate
                };

                _context.Bookings.Add(booking);
                //Console.WriteLine($"TrekId: {model.TrekId}, StartDate: {model.StartDate}, BookingDate: {DateTime.Now}");
                Console.WriteLine($"BookingDate: {booking.BookingDate}");
                //_context.Entry(booking).State = EntityState.Modified;
                _context.SaveChanges();

                //_context.SaveChanges();

                return View("PaymentPage", booking);
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

            // Assume the payment is successful. In a real application, integrate with a payment gateway.
            var payment = new Payment
            {
                BookingId = bookingId,
                Amount = booking.TotalAmount,
                PaymentDate = DateTime.Now,
                PaymentMethod = paymentMethod
            };

            _context.Payments.Add(payment);
            _context.SaveChanges();

            // Update booking status (optional)
            // booking.Status = "Paid";
            // _context.SaveChanges();

            return RedirectToAction("BookingDetails", new { bookingId = bookingId });
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
            var cancellation = new Cancellation
            {
                BookingId = model.BookingId,
                Reason = model.Reason,
                RefundAmount = refundAmount,
                CancellationDate = DateTime.Now
            };

            _context.Cancellations.Add(cancellation);

            // Remove the booking from the Bookings table
            _context.Bookings.Remove(book);

            await _context.SaveChangesAsync();

            // Redirect to MyBookings page
            return RedirectToAction("MyBookings", "Profile");
        }




    }
}
