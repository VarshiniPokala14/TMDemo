using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TrekMasters.Repository;
using TrekMasters.Service;

namespace TrekMasters.Service
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;
        public AdminService(IAdminRepository adminRepository,ICategoryRepository categoryRepository,IBookingRepository bookingRepository,IEmailSender emailSender,IMemoryCache cache)
        {
            _adminRepository = adminRepository;
            _categoryRepository = categoryRepository;
            _bookingRepository = bookingRepository;
            _emailSender = emailSender;
            _cache = cache;
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
            await _adminRepository.AddTrekAsync(trek);
            

           
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

                await _adminRepository.AddTrekPlanAsync(trekPlan);
            }
        }

        //public async Task AddAvailabilityAsync(Availability availability)
        //{
        //    await   _adminRepository.AddAvailabilityAsync(availability);
        //}

        public async Task<List<Trek>> GetAllTreksAsync()
        {
            
            return await _adminRepository.GetAllTreksAsync();
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
            return _bookingRepository.GetTrekById(trekId);
        }
        public Availability GetAvailabilityById(int availabilityId)
        {
            return  _adminRepository.GetAvailabilityById(availabilityId);
        }
        public async Task AddAvailabilityAsync(int trekId, DateTime startDate, DateTime endDate,string month,int maxGroup)
        {
            var trek = _bookingRepository.GetTrekById(trekId);
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
            await _adminRepository.AddAvailabilityAsync(availability);
            _cache.Remove("Availabilities:All");
            if (isFirstAvailability)
            {
                await NotifyUsersAboutAvailabilityAsync(trekId);
            }
        }

        private async Task NotifyUsersAboutAvailabilityAsync(int trekId)
        {
            var trek = _bookingRepository.GetTrekById(trekId);
            
            var notificationRequests = await _adminRepository.GetNotificationRequestsAsync(trekId);
            foreach (var request in notificationRequests)
            {
                await _emailSender.SendEmailAsync(request.Email, "New Trek Availability", $"Good News! New availability added for trek {trek.Name}");
            }
            
            await _adminRepository.RemoveNotificationRequests(notificationRequests);
        }
    }

}
