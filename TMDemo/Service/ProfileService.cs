﻿using Microsoft.Extensions.Caching.Memory;
using TMDemo.Repository;

namespace TMDemo.Service
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

            // Check if the user is in cache
            if (!_cache.TryGetValue(cacheKey, out UserDetail user))
            {
                // If not in cache, retrieve from DB
                user = await _userRepository.GetUserAsync(userId);

                // Add to cache with expiration
                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(30));
            }

            return user;
        }

        public async Task<bool> UpdateUserAsync(UserDetail user)
        {
            var result = await _userRepository.UpdateUserAsync(user);

            if (result)
            {
                // Update the cache after successful DB update
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

            // Update cache
            var cacheKey = $"EmergencyContact_{userId}";
            _cache.Set(cacheKey, emergencyContact, TimeSpan.FromMinutes(30));
        }

        public async Task<List<Booking>> GetBookingsByUserIdAsync(string userId)
        {
            var cacheKey = $"Bookings_{userId}";

            if (!_cache.TryGetValue(cacheKey, out List<Booking> bookings))
            {
                bookings = await _userRepository.GetBookingsByUserIdAsync(userId);
                _cache.Set(cacheKey, bookings, TimeSpan.FromMinutes(30));
            }

            return bookings;
        }
    }
}