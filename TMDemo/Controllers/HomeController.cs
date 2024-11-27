using Microsoft.AspNetCore.Mvc;


namespace TMDemo.Controllers
{
    public class HomeController : Controller
    {
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
    }
}