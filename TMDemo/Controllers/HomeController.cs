
namespace TrekMasters.Controllers
{
    public class HomeController : Controller
    {

        private readonly UserManager<UserDetail> _userManager;
        private readonly INotificationService _notificationService;

        public HomeController(UserManager<UserDetail> userManager, INotificationService notificationService)
        {
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Cancellation()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult FAQs()
        {
            return View();
        }
        [Route("Error/{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return View("404");
                case 500:
                    return View("500");
                case 403:
                    return View("403");
                default:
                    return View("GenericError");
            }
        }
        public async Task<IActionResult> Route()
        {
            UserDetail user = await _userManager.GetUserAsync(User);
            List<String> Roles = User.FindAll(ClaimTypes.Role).Select(r =>r.Value).ToList();
            if (Roles.Contains("Admin"))
            {
                return RedirectToAction("Treks", "Admin");
            }
            return View("Index");
        }
        public async Task<IActionResult> Notification()
        {
            var userId = _userManager.GetUserId(User);
            var notifications = await _notificationService.GetNotificationsForUserAsync(userId);
            ViewData["Notifications"] = notifications;

            return View();
        }

    }
}