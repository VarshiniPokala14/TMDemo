namespace TrekMasters.Service
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;
        private readonly UserManager<UserDetail> _userManager;
        public AdminService(IAdminRepository adminRepository,ICategoryRepository categoryRepository,IBookingRepository bookingRepository,IEmailSender emailSender,IMemoryCache cache,IUserRepository userRepository,INotificationService notificationService,UserManager<UserDetail> userManager)
        {
            _adminRepository = adminRepository;
            _categoryRepository = categoryRepository;
            _bookingRepository = bookingRepository;
            _emailSender = emailSender;
            _cache = cache;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        
        public async Task<int> AddTrekAsync(AddTrekViewModel model)
        {
            var trek = new Trek
            {
                Name = model.Name,
                Region = model.Region,
                Description = model.Description,
                DifficultyLevel = model.DifficultyLevel,
                DurationDays = model.DurationDays,
                HighAltitude = model.HighAltitude,
                Price = model.Price,
                Season = model.SelectedSeasons
            };

            if (model.TrekImgFile != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await model.TrekImgFile.CopyToAsync(memoryStream);
                    trek.TrekImg = memoryStream.ToArray();
                }
            }
            await _adminRepository.AddAsync<Trek>(trek);
            

           
            return trek.TrekId;
        }

        public async Task AddTrekPlanAsync(TrekPlanViewModel model)
        {
            foreach (var activity in model.Activities)
            {
                TrekPlan trekPlan = new TrekPlan
                {
                    TrekId = model.TrekId,
                    Day = activity.Day,
                    ActivityDescription = activity.ActivityDescription
                };

                await _adminRepository.AddAsync<TrekPlan>(trekPlan);
            }
        }

        
        public async Task<List<Trek>> GetAllTreksAsync()
        {
            
            return await _adminRepository.GetAllAsync<Trek>();
        }

        public async Task<List<Availability>> GetAllAvailabilitiesAsync()
        {
            return await _categoryRepository.GetAllAvailabilitiesAsync();
        }

        public async Task<List<TrekAvailabilityViewModel>> GetTrekBookingsAsync(int trekId)
        {
            return await _adminRepository.GetTrekBookingsAsync(trekId);
        }

        public async Task<List<UserViewModel>> GetUsersForAvailabilityAsync(int availabilityId)
        {
            return await _adminRepository.GetUsersForAvailabilityAsync(availabilityId);
        }

        public async Task<Trek> GetTrekByIdAsync(int trekId)
        {
            return   _bookingRepository.GetTrekById(trekId);
        }
        public Availability GetAvailabilityById(int availabilityId)
        {
            return  _adminRepository.GetAvailabilityById(availabilityId);
        }
        public async Task AddAvailabilityAsync(int trekId, DateTime startDate, DateTime endDate,string month,int maxGroup)
        {
            var trek =_bookingRepository.GetTrekById(trekId);   
            if (trek == null)
                throw new ArgumentException("Trek not found");

            var availability = new Availability
            {
                TrekId = trekId,
                StartDate = startDate,
                EndDate = endDate,
                Month = month,
                MaxGroupSize=maxGroup
            };
            bool isFirstAvailability = !await _adminRepository.AvailabilityExistsAsync(trekId);
            await _adminRepository.AddAsync<Availability>(availability);
            _cache.Remove("Availabilities:All");
            if (isFirstAvailability)
            {
                await NotifyUsersAboutAvailabilityAsync(trekId);
            }
        }

        private async Task NotifyUsersAboutAvailabilityAsync(int trekId)
        {
            string myUserId = _userRepository.GetCurrentUserId();

            var trek =_bookingRepository.GetTrekById(trekId);
            var notificationRequests = await _adminRepository.GetNotificationRequestsAsync(trekId);
            foreach (var request in notificationRequests)
            {
                var user =await _userManager.FindByEmailAsync(request.Email);
                if (user != null)
                {
                    string body= $"Good News {user.FirstName} {user.LastName}! New availability added for trek {trek.Name}";
                    await _notificationService.CreateNotificationAsync(myUserId, user.Id, body);
                }
                await _emailSender.SendEmailAsync(request.Email, "New Trek Availability", $"Good News! New availability added for trek {trek.Name}");
            }
            foreach (var request in notificationRequests)
            {
                await _adminRepository.DeleteAsync<NotificationRequest>(request);
            }
        }
        public async Task<bool> CheckAvailabilityConflictAsync(int trekId, DateTime startDate, DateTime endDate)
        {
            var conflictingAvailability = await _adminRepository.GetConflictingAvailabilityAsync(trekId, startDate, endDate);
            return conflictingAvailability != null;
        }
        public async Task<IEnumerable<TrekParticipant>> GetParticipantsForBookingAsync(int bookingId)
        {
            if (bookingId <= 0)
            {
                throw new ArgumentException("Invalid booking ID.", nameof(bookingId));
            }

            return await _adminRepository.GetParticipantsByBookingIdAsync(bookingId);
        }
        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            if (bookingId <= 0)
            {
                throw new ArgumentException("Invalid booking ID", nameof(bookingId));
            }

            return await _adminRepository.GetBookingByIdAsync(bookingId);
        }

        public async Task<TrekCancellationViewModel> GetTrekCancellationViewModelAsync(int availabilityId)
        {
            var availability = await _adminRepository.GetAvailabilityByIdAsync(availabilityId);
            var bookings = await _adminRepository.GetBookingsByAvailabilityIdAsync(availabilityId);

            if (availability == null || bookings == null)
                throw new Exception("Invalid availability or no bookings found.");

            return new TrekCancellationViewModel
            {
                AvailabilityId = availability.AvailabilityId,
                TrekName = availability.Trek.Name,
                StartDate = availability.StartDate,
                TotalUsers = bookings.Count,
                TotalRefundAmount = bookings.Sum(b => b.TotalAmount),
            };
        }

        public async Task ProcessTrekCancellationAsync(TrekCancellationViewModel model)
        {
            string myUserId = _userRepository.GetCurrentUserId();
            var bookings = await _adminRepository.GetBookingsByAvailabilityIdAsync(model.AvailabilityId);
            var emailSet = new HashSet<string>();
            var userIdset=new HashSet<string>();

            foreach (var booking in bookings)
            {
                booking.IsCancelled = true;
                booking.CancellationDate = DateTime.Now;
                booking.RefundAmount = booking.TotalAmount;
                booking.Reason = model.Reason;
                if (!string.IsNullOrEmpty(booking.User?.Email))
                {
                    emailSet.Add(booking.User.Email);
                }
                if (!string.IsNullOrEmpty(booking.User?.Id))
                {
                    userIdset.Add(booking.User.Id);
                }
                if (booking.TrekParticipants != null)
                {
                    foreach (var participant in booking.TrekParticipants)
                    {
                        if (!string.IsNullOrEmpty(participant.Email))
                        {
                            emailSet.Add(participant.Email);
                        }
                        
                    }
                }

                await _adminRepository.UpdateAsync(booking);
            }
            var availability = await _adminRepository.GetAvailabilityByIdAsync(model.AvailabilityId);
            if (availability != null)
            {
                await _adminRepository.DeleteAsync<Availability>(availability);
            }
            var trekName = availability?.Trek?.Name ?? "the trek"; 
            var subject = $"Cancellation of Trek: {trekName}";
            var body = $"We regret to inform you that the trek '{trekName}' starting on {availability?.StartDate:yyyy-MM-dd} has been cancelled. You will receive a full refund soon.";

            foreach (var email in emailSet)
            {
                await _emailSender.SendEmailAsync(email, subject, body);
            }
            foreach (var userId in userIdset)
            {
               await _notificationService.CreateNotificationAsync(myUserId, userId,body);
            }
        }
    }

}
