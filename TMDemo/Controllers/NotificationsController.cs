namespace TrekMasters.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<UserDetail> _userManager;

        public NotificationController(INotificationService notificationService, UserManager<UserDetail> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var notifications = await _notificationService.GetNotificationsForUserAsync(userId);
            return View(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            if (notificationId > 0)
            {
                await _notificationService.MarkAsReadAsync(notificationId);

            }
            return Ok();
        }
    }
}
