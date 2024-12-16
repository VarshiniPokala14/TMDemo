
namespace TrekMasters.Service
{
    public class TrekService : ITrekService
    {
        private readonly ITrekRepository _trekRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMemoryCache _cache;
        private readonly IAvailabilityRepository _availabilityRepository;

        public TrekService(ITrekRepository trekRepository, IUserRepository userRepository, ICategoryRepository categoryRepository,IMemoryCache cache,IAvailabilityRepository availabilityRepository)
        {
            _trekRepository = trekRepository;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _cache = cache;
            _availabilityRepository = availabilityRepository;
        }

        public async Task<List<Trek>> GetAllTreksAsync()
        {
            return await _categoryRepository.GetAllTreksWithDetailsAsync();
        }

        public async Task<List<Trek>> GetUpcomingTreksAsync()
        {
            DateTime today = DateTime.Today;
            DateTime threeMonthsLater = today.AddMonths(3);
            return await _trekRepository.GetUpcomingTreksAsync(today, threeMonthsLater);
        }

        public async Task<TrekDetailsViewModel> GetTrekDetailsAsync(int trekId, string userId)
        {
            var trek = await _trekRepository.GetTrekDetailsAsync(trekId);
            if (trek == null) return null;

            await CleanupPastAvailabilitiesAsync();
            _cache.Remove("Availabilities:All");

            var currentDate = DateTime.Now;

            var userBooking = await _trekRepository.GetUserBookingAsync(userId, trekId);

            DateTime trekEndDate = userBooking?.TrekStartDate.AddDays(trek.DurationDays) ?? DateTime.MinValue;
            bool isTrekCompleted = trekEndDate <= currentDate;
            bool hasReviewed = trek.TrekReviews.Any(r => r.UserId == userId);

            var availabilityDates = new List<MonthAvailability>();
            foreach (var group in trek.Availabilities
                .OrderBy(date => date.StartDate.Year)
                .ThenBy(date => date.StartDate.Month)
                .GroupBy(a => a.StartDate.ToString("MMMM yyyy")))
            {
                var dates = new List<DateRange>();
                foreach (var a in group)
                {
                    var remainingSlots = await _trekRepository.GetRemainingSlotsAsync(trek.TrekId, a.StartDate);
                    dates.Add(new DateRange
                    {
                        StartDate = a.StartDate,
                        EndDate = a.EndDate,
                        RemainingSlots = remainingSlots
                    });
                }
                availabilityDates.Add(new MonthAvailability { Month = group.Key, Dates = dates });
            }

            return new TrekDetailsViewModel
            {
                Trek = trek,
                AvailabilityDates = availabilityDates,
                Reviews = trek.TrekReviews.OrderByDescending(r => r.CreatedAt).ToList(),
                TrekPlan = trek.TrekPlans.OrderBy(tp => tp.Day).Select(tp => tp.ActivityDescription).ToList(),
                Bookings = trek.Bookings.ToList(),
                IsTrekCompleted = isTrekCompleted,
                HasReviewed = hasReviewed
            };
        }
        public async Task<List<Trek>> SearchTreksAsync(string searchString)
        {
            return await _trekRepository.SearchTreksAsync(searchString);
        }
        public async Task CleanupPastAvailabilitiesAsync()
        {
            var pastAvailabilities = await _availabilityRepository.GetPastAvailabilitiesAsync();
            if (pastAvailabilities.Any())
            {
                //await _trekRepository.RemovePastAvailabilitiesAsync(pastAvailabilities);
                foreach (var availability in pastAvailabilities)
                {
                    await _availabilityRepository.DeleteAsync<Availability>(availability);
                }
            }
        }
        public async Task<byte[]> GetTrekImageAsync(int trekId)
        {
            return await _trekRepository.GetTrekImageAsync(trekId);
        }
        [Authorize]
        public async Task<string> AddReviewAsync( int trekId, string reviewText)
        {
            var userId = _userRepository.GetCurrentUserId();
            var booking = await _trekRepository.GetUserBookingAsync(userId, trekId);

            if (booking == null)
            {
                return "You must book the trek first.";
            }

            DateTime trekEndDate = booking.TrekStartDate.AddDays(booking.Trek.DurationDays);

            if (trekEndDate > DateTime.Now)
            {
                return "You can only leave a review after completing the trek.";
            }

            bool reviewExists = await _trekRepository.ReviewExistsAsync(userId, trekId);

            if (reviewExists)
            {
                return "You have already reviewed this trek.";
            }

            TrekReview review = new TrekReview
            {
                UserId = userId,
                TrekId = trekId,
                Comment = reviewText,
                CreatedAt = DateTime.Now
            };

            //await _trekRepository.AddReviewAsync(review);
            await _trekRepository.AddAsync<TrekReview>(review);

            return null;
        }

        public async Task AddNotificationRequestAsync(int trekId, string email)
        {
            var notificationRequest = new NotificationRequest
            {
                TrekId = trekId,
                Email = email,
                RequestedOn = DateTime.Now
            };

            // Save the request to the database
            _trekRepository.AddAsync<NotificationRequest>(notificationRequest);

        }

        public async Task<NotificationRequest> GetNotificationRequestAsync(int trekId, string email)
        {
            return await _trekRepository.GetNotificationRequestAsync(trekId, email);
        }


    }
}
