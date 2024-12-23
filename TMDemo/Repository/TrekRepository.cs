﻿using Microsoft.EntityFrameworkCore;

namespace TrekMasters.Repository
{

    public class TrekRepository : Repository,ITrekRepository
    {
        private readonly AppDbContext _context;
        public TrekRepository(AppDbContext context) : base(context) {
            _context= context;
        }

        public async Task<List<Trek>> GetUpcomingTreksAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Treks
                .Include(t => t.Availabilities)
                .Where(t => t.Availabilities.Any(a => a.StartDate >= startDate && a.StartDate <= endDate))
                .ToListAsync();
        }

        public async Task<byte[]> GetTrekImageAsync(int trekId)
        {
            return await _context.Treks
                .Where(t => t.TrekId == trekId)
                .Select(t => t.TrekImg)
                .FirstOrDefaultAsync();
        }

        public async Task<Trek> GetTrekDetailsAsync(int trekId)
        {
            return await _context.Treks
                .Include(t => t.Availabilities)
                .Include(t => t.TrekReviews)
                    .ThenInclude(r => r.User)
                .Include(t => t.Bookings)
                    .ThenInclude(b => b.User)
                .Include(t => t.TrekPlans)
                .FirstOrDefaultAsync(t => t.TrekId == trekId);
        }



        public async Task<int> GetRemainingSlotsAsync(int trekId, DateTime startDate)
        {
            var totalSlots = await _context.Availabilities
                .Where(a => a.TrekId == trekId && a.StartDate == startDate)
                .Select(a => a.MaxGroupSize)
            .FirstOrDefaultAsync();

            var totalBookedSlots = await _context.Bookings
                .Where(b => b.TrekId == trekId && b.TrekStartDate == startDate && b.PaymentSuccess==true)
                .SumAsync(b => b.NumberOfPeople);

            var totalCancelledSlots = await _context.Bookings
                .Where(b => b.TrekId == trekId && b.TrekStartDate == startDate && b.IsCancelled == true && b.PaymentSuccess == true)
                .SumAsync(b => b.NumberOfPeople);

            return totalSlots - (totalBookedSlots - totalCancelledSlots);
        }
        public async Task<NotificationRequest> GetNotificationRequestAsync(int trekId, string email)
        {
            return await _context.NotificationRequests
                                  .FirstOrDefaultAsync(nr => nr.TrekId == trekId && nr.Email == email);
        }
        public async Task<List<Trek>> SearchTreksAsync(string searchString)
        {
            var query = _context.Treks.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(t => t.Name.ToUpper().Contains(searchString.ToUpper()));
            }

            return await query.ToListAsync();
        }
        public async Task<Booking> GetUserBookingAsync(string userId, int trekId)
        {
            return await _context.Bookings
                .Include(t => t.Trek)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.TrekId == trekId &&
                                          (b.IsCancelled == false || b.IsCancelled == null));
        }

        public async Task<bool> ReviewExistsAsync(string userId, int trekId)
        {
            return await _context.TrekReviews
                .AnyAsync(r => r.UserId == userId && r.TrekId == trekId);
        }
        public async Task<string> GetAdminUserIdAsync()
        {
            // Retrieve the Role ID for the Admin role
            var adminRoleId = await _context.Roles
                .Where(r => r.Name == "Admin")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(adminRoleId))
            {
                throw new Exception("Admin role not found.");
            }

            // Retrieve the User ID of a user with the Admin role
            var adminUserId = await _context.UserRoles
                .Where(ur => ur.RoleId == adminRoleId)
                .Select(ur => ur.UserId)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(adminUserId))
            {
                throw new Exception("No user with Admin role found.");
            }

            return adminUserId;
        }

    }

}
