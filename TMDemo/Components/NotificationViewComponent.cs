//namespace TrekMasters.Components
//{
//    public class NotificationViewComponent : ViewComponent
//    {
//        private readonly INotificationService _notificationService;
//        private readonly UserManager<UserDetail> _userManager;

//        public NotificationViewComponent(INotificationService notificationService, UserManager<UserDetail> userManager)
//        {
//            _notificationService = notificationService;
//            _userManager = userManager;
//        }

//        public async Task<IViewComponentResult> InvokeAsync()
//        {
//            // Get the current user ID
//            var userId = _userManager.GetUserId(HttpContext.User);

//            // Fetch notifications for the user
//            var notifications = await _notificationService.GetNotificationsForUserAsync(userId);

//            // Prepare the notification data for the view (you can also process this if needed)
//            var model = new NotificationViewModel
//            {
//                Notifications = notifications,
//                UnreadCount = notifications.Count(n => !n.IsRead) // Count unread notifications
//            };

//            return View(model); // Return a view that will be rendered inside the layout
//        }
//    }

//}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace TrekMasters.Components
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<UserDetail> _userManager;

        public NotificationViewComponent(INotificationService notificationService, UserManager<UserDetail> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var notifications = await _notificationService.GetNotificationsForUserAsync(userId);

            var model = new NotificationViewModel
            {
                Notifications = notifications,
                UnreadCount = notifications.Count(n => !n.IsRead)
            };

            return View(model);
        }
    }
}
