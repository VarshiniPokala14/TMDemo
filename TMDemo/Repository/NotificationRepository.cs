//using Microsoft.AspNetCore.Identity;

//namespace TrekMasters.Repository
//{
//    public class NotificationRepository : INotificationRepository
//    {
//        private readonly AppDbContext _context;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private readonly UserManager<UserDetail> _userManager;  
//        public NotificationRepository(AppDbContext context, RoleManager<IdentityRole> roleManager, UserManager<UserDetail> userManager)
//        {
//            _context = context;
//            _roleManager = roleManager;
//            _userManager = userManager;
//        }
//        public void Create(Notification notification)
//        {
//            _context.Notifications.Add(notification);
//            _context.SaveChanges();
//            var adminrole = _context.Roles.FirstOrDefault(r => r.Name == "Admin");
//            // Extract the user IDs
//            var adminUser=_context.UserRoles.FirstOrDefault(ur=>ur.Equals(adminrole));
            

//            // Use UserManager to get all users in the Admin role
//            var adminUsers = _userManager.GetUsersInRoleAsync("Admin");
            
//            var notificationRequest = new NotificationRequest
//            {
//                NotificationId = notification.Id,
//                AdminUserId = adminUser.UserId,
//                Notification = notification,
//                IsRead = false
//            };

//            _context.NotificationRequests.Add(notificationRequest);
            
//            _context.SaveChanges();

//        }
//        public List<NotificationRequest> GetUserNotifications(string userId)
//        {
//            return _context.NotificationRequests
//                           .Where(nr => nr.AdminUserId.Equals(userId) && !nr.IsRead) // Filter by AdminUserId and unread notifications
//                           .Include(nr => nr.Notification) // Include related Notification details
//                           .ToList();
//        }
//        public void ReadNotification(int id,string userId)
//        {
//            var notification = _context.NotificationRequests.FirstOrDefault(n=>n.NotificationId == id);
//            notification.IsRead = true;
//            _context.NotificationRequests.Update(notification);
//            _context.SaveChanges();

//        }


//    }
//}
