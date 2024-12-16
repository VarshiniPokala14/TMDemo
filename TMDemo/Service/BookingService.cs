namespace TrekMasters.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMemoryCache _cache;
        private readonly IUserRepository _userRepository;
        private readonly ITrekRepository _trekRepository;
        public BookingService(IBookingRepository bookingRepository,IMemoryCache cache, IUserRepository userRepository, ITrekRepository trekRepository)
        {
            _bookingRepository = bookingRepository;
            _cache = cache;
            _userRepository = userRepository;
            _trekRepository = trekRepository;
        }
        public async Task<(Booking,List<string>)> CreateBookingAsync(AddUsersViewModel model, string userId)
        {
            Trek trek = _bookingRepository.GetTrekById(model.TrekId);
            if (trek == null)
            {
                throw new KeyNotFoundException("Trek not found.");
            }
            int remainingSlots = await _trekRepository.GetRemainingSlotsAsync(model.TrekId, model.StartDate);

            if (model.Participants.Count > remainingSlots)
            {
                throw new InvalidOperationException($"Only {remainingSlots} slots are available for this trek.");
            }
            List<string> conflictWarnings = new List<string>();
            foreach (var participant in model.Participants)
            {
                var overlappingBookings = await _bookingRepository.GetOverlappingBookingsAsync(participant.Email, model.StartDate, model.StartDate.AddDays(trek.DurationDays));

                if (overlappingBookings.Any())
                {
                    string conflicts = string.Join(", ", overlappingBookings.Select(b => $"Booking ID: {b.BookingId}, Trek: {b.Trek.Name} from {b.TrekStartDate} to {b.TrekStartDate.AddDays(b.Trek.DurationDays-1)}"));
                    conflictWarnings.Add($"Participant {participant.Name} ({participant.Email}) has overlapping bookings: {conflicts}.");
                }
            }
            decimal amount = trek.Price * model.Participants.Count;
            decimal tax = amount * 0.05M;
            decimal totalAmount = amount + tax + 10;

            var booking = new Booking
            {
                TrekId = model.TrekId,
                UserId = userId,
                BookingDate = DateTime.Now,
                NumberOfPeople = model.Participants.Count,
                TotalAmount = totalAmount,
                TrekStartDate = model.StartDate,
                CancellationDate = DateTime.MinValue
            };

            await _bookingRepository.AddAsync<Booking>(booking);
            string cacheKey = $"Bookings_{userId}";
            _cache.Remove(cacheKey);
            return (booking, conflictWarnings);
        }
        public async Task<bool> ProcessPayment(int bookingId, string paymentMethod)
        {
           
            var booking = _bookingRepository.GetBookingById(bookingId);
            if (booking == null)
            {
                return false; 
            }
            booking.PaymentSuccess = true;

            
            await _bookingRepository.UpdateAsync<Booking>(booking);

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

        public async Task ProcessCancellation(CancellationViewModel model)
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

           var result =   await _bookingRepository.UpdateAsync<Booking>(booking);
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

        public async Task ProcessReschedule(RescheduleViewModel model)
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
                await _bookingRepository.UpdateAsync<Booking>(booking);
                string cacheKey = $"Bookings_{userId}";
                _cache.Remove(cacheKey);
            }

        }
        public AddUsersViewModel GetAddUsersViewModel(int trekId,string startDate, string userEmail)
        {
            Trek trek = _bookingRepository.GetTrekById(trekId);
            if (trek == null)
            {
                throw new KeyNotFoundException("Trek not found.");
            }

            return new AddUsersViewModel
            {
                TrekId = trekId,
                TrekName = trek.Name,
                StartDate = DateTime.Parse(startDate),
                Participants = new List<ParticipantViewModel>()
            };
            
        }
        public async Task AddParticipant(int bookingId, ParticipantViewModel participant)
        {
            var trekParticipant = new TrekParticipant
            {
                BookingId = bookingId,
                Email = participant.Email,
                Name = participant.Name,
                ContactNumber = participant.ContactNumber
            };

             await _bookingRepository.AddAsync<TrekParticipant>(trekParticipant);
        }

    }

}
