using Microsoft.AspNetCore.Identity;

namespace TrekMasters.Repository
{
    public class NotificationRepository : Repository,INotificationRepository
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<UserDetail> _userManager;
        public NotificationRepository(AppDbContext context, RoleManager<IdentityRole> roleManager, UserManager<UserDetail> userManager):base(context)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<List<Notification>> GetNotificationsForUserAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.RecipientId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
        public Notification GetNotificationById(int Id)
        {
            return  _context.Notifications.FirstOrDefault(n=>n.NotificationId==Id);
        }
        public async Task<List<Notification>> GetUnreadNotificationsForUserAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.RecipientId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }


    }
}
