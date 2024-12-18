//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;

//namespace TrekMasters.Controllers
//{

//    [Route("Notifications")]
//    public class NotificationsController : ControllerBase
//    {
//        private INotificationRepository _notificationRepository;
//        private UserManager<UserDetail> _userManager;

//        public NotificationsController(INotificationRepository notificationRepository, UserManager<UserDetail> userManager)
//        {
//            _notificationRepository = notificationRepository;
//            _userManager = userManager;
//        }
//        public IActionResult GetNotification()
//        {
//            var userId = _userManager.GetUserId(HttpContext.User);
//            var notification = _notificationRepository.GetUserNotifications(userId);
//            return Ok(new { UserNotification = notification, Count = notification.Count });
//        }

//        public IActionResult ReadNotification(int notificationId)
//        {

//            _notificationRepository.ReadNotification(notificationId, _userManager.GetUserId(HttpContext.User));

//            return Ok();
//        }





//    }

//}
