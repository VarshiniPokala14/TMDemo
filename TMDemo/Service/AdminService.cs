using Microsoft.EntityFrameworkCore;
using TMDemo.Repository;
using TMDemo.Service;

namespace TMDemo.Service
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookingRepository _bookingRepository;
        public AdminService(IAdminRepository adminRepository,ICategoryRepository categoryRepository,IBookingRepository bookingRepository)
        {
            _adminRepository = adminRepository;
            _categoryRepository = categoryRepository;
            _bookingRepository = bookingRepository;
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

        public async Task AddAvailabilityAsync(Availability availability)
        {
            await   _adminRepository.AddAvailabilityAsync(availability);
        }

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
    }

}
