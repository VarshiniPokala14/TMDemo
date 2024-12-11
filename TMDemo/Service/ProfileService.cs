
namespace TrekMasters.Service
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _cache;

        public ProfileService(IUserRepository userRepository, IMemoryCache cache)
        {
            _userRepository = userRepository;
            _cache = cache;
        }

        public async Task<UserDetail> GetUserAsync(string userId)
        {
            var cacheKey = $"UserDetail_{userId}";

            if (!_cache.TryGetValue(cacheKey, out UserDetail user))
            {
                user = await _userRepository.GetUserAsync(userId);

                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(30));
            }

            return user;
        }

        public async Task<bool> UpdateUserAsync(UserDetail user)
        {
            var result = await _userRepository.UpdateUserAsync(user);

            if (result)
            {
                var cacheKey = $"UserDetail_{user.Id}";
                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(30));
            }

            return result;
        }

        public async Task<EmergencyContact> GetEmergencyContactAsync(string userId)
        {
            var cacheKey = $"EmergencyContact_{userId}";

            if (!_cache.TryGetValue(cacheKey, out EmergencyContact emergencyContact))
            {
                emergencyContact = await _userRepository.GetEmergencyContactAsync(userId);
                _cache.Set(cacheKey, emergencyContact, TimeSpan.FromMinutes(30));
            }

            return emergencyContact;
        }

        public async Task AddOrUpdateEmergencyContactAsync(string userId, EmergencyContact emergencyContact)
        {
            var existingContact = await GetEmergencyContactAsync(userId);

            if (existingContact == null)
            {
                emergencyContact.UserId = userId;
                await _userRepository.AddEmergencyContactAsync(emergencyContact);
            }
            else
            {
                existingContact.ContactName = emergencyContact.ContactName;
                existingContact.ContactNumber = emergencyContact.ContactNumber;
                existingContact.Email = emergencyContact.Email;
                existingContact.Relation = emergencyContact.Relation;
                await _userRepository.UpdateEmergencyContactAsync(existingContact);
            }

            var cacheKey = $"EmergencyContact_{userId}";
            _cache.Set(cacheKey, emergencyContact, TimeSpan.FromMinutes(30));
        }
        public async Task<MyBookingsViewModel> GetBookingsByUserIdAsync(string userId)
        {
            var cacheKey = $"Bookings_{userId}";

            if (!_cache.TryGetValue(cacheKey, out List<Booking> bookings))
            {
                bookings = await _userRepository.GetBookingsByUserIdAsync(userId);
                _cache.Set(cacheKey, bookings, TimeSpan.FromMinutes(30));
            }

            var completedBookings = bookings
                .Where(b => b.TrekStartDate.AddDays(b.Trek.DurationDays - 1) <= DateTime.Now && (b.IsCancelled == false || b.IsCancelled == null))
                .ToList();

            var incompleteBookings = bookings
                .Where(b => b.TrekStartDate.AddDays(b.Trek.DurationDays - 1) > DateTime.Now && (b.IsCancelled == false || b.IsCancelled == null))
                .ToList();

            return new MyBookingsViewModel
            {
                CompletedBookings = completedBookings,
                IncompleteBookings = incompleteBookings
            };
        }
    }
}
