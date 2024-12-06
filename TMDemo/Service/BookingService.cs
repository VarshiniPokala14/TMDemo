using Microsoft.Extensions.Caching.Memory;
using TMDemo.Models;
using TMDemo.Repository;

namespace TMDemo.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMemoryCache _cache;
        private readonly IUserRepository _userRepository;
        public BookingService(IBookingRepository bookingRepository,IMemoryCache cache, IUserRepository userRepository)
        {
            _bookingRepository = bookingRepository;
            _cache = cache;
            _userRepository = userRepository;
        }

        public AddUsersViewModel GetAddUsersViewModel(int trekId, string startDate, string userEmail)
        {
            DateTime parsedDate;
            if (!DateTime.TryParse(startDate, out parsedDate))
            {
                throw new ArgumentException("Invalid date format.");
            }

            Trek trek = _bookingRepository.GetTrekById(trekId);
            if (trek == null)
            {
                throw new KeyNotFoundException("Trek not found.");
            }

            List<string> emails = new List<string> { userEmail };

            return new AddUsersViewModel
            {
                TrekId = trekId,
                TrekName = trek.Name,
                StartDate = parsedDate,
                Emails = emails
            };
        }

        public void AddMember(AddUsersViewModel model, string email)
        {
            if (model != null && !string.IsNullOrEmpty(email))
            {
                model.Emails.Add(email);
            }
        }

        public Booking CreateBooking(AddUsersViewModel model, string userId)
        {
            Trek trek = _bookingRepository.GetTrekById(model.TrekId);
            if (trek == null)
            {
                throw new KeyNotFoundException("Trek not found.");
            }

            decimal amount = trek.Price * model.Emails.Count;
            decimal tax = amount * 0.05M;
            decimal totalAmount = amount + tax + 10;

            var booking = new Booking
            {
                TrekId = model.TrekId,
                UserId = userId,
                BookingDate = DateTime.Now,
                NumberOfPeople = model.Emails.Count,
                TotalAmount = totalAmount,
                TrekStartDate = model.StartDate,
                CancellationDate = DateTime.MinValue
            };

            _bookingRepository.AddBooking(booking);
            _bookingRepository.Save();
            string cacheKey = $"Bookings_{userId}";
            _cache.Remove(cacheKey);
            return booking;
        }
        public async Task<bool> ProcessPayment(int bookingId, string paymentMethod)
        {
            // Retrieve the booking from the repository
            var booking = _bookingRepository.GetBookingById(bookingId);
            if (booking == null)
            {
                return false; 
            }

            
            booking.PaymentSuccess = true;

            
            _bookingRepository.UpdateBooking(booking);
            _bookingRepository.Save();

            return true; 
        }
        public CancellationViewModel GetCancellationViewModel(int bookingId)
        {
            Booking booking = _bookingRepository.GetBookingById(bookingId);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found.");
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

            return new CancellationViewModel
            {
                BookingId = booking.BookingId,
                Booking = booking,
                RefundAmount = refundAmount
            };
        }

        public void ProcessCancellation(CancellationViewModel model)
        {
            var userId = _userRepository.GetCurrentUserId();
            Booking booking = _bookingRepository.GetBookingById(model.BookingId);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found.");
            }

            booking.IsCancelled = true;
            booking.Reason = model.Reason;
            booking.RefundAmount = model.RefundAmount;
            booking.CancellationDate = DateTime.Now;

            _bookingRepository.UpdateBooking(booking);
            _bookingRepository.Save();
            string cacheKey = $"Bookings_{userId}";
            _cache.Remove(cacheKey);
        }

        public RescheduleViewModel GetRescheduleViewModel(int bookingId)
        {
            Booking booking = _bookingRepository.GetBookingById(bookingId);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found.");
            }

            var availableDates = _bookingRepository.GetAvailableDates(booking.TrekId);
            if (availableDates == null || !availableDates.Any())
            {
                throw new InvalidOperationException("No available dates for this trek.");
            }

            var daysDifference = (booking.TrekStartDate - DateTime.Now).Days;
            decimal extraAmount = 0;
            if (daysDifference <= 25)
            {
                extraAmount = booking.TotalAmount * 0.05M;
            }

            return new RescheduleViewModel
            {
                BookingId = booking.BookingId,
                OldStartDate = booking.TrekStartDate,
                AvailableDates = availableDates,
                ExtraAmount = extraAmount
            };
        }

        public void ProcessReschedule(RescheduleViewModel model)
        {
            var userId = _userRepository.GetCurrentUserId();
            Booking booking = _bookingRepository.GetBookingById(model.BookingId);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found.");
            }

            booking.ExtraAmount = model.ExtraAmount;
            booking.RescheduleReason = model.Reason;
            booking.TrekStartDate = model.NewStartDate;
            booking.TotalAmount += model.ExtraAmount;
            if (model.OldStartDate != model.NewStartDate)
            {
                _bookingRepository.UpdateBooking(booking);
                _bookingRepository.Save();
                string cacheKey = $"Bookings_{userId}";
                _cache.Remove(cacheKey);
            }
        }
    }

}
