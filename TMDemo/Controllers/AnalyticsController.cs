using Microsoft.AspNetCore.Mvc;

namespace TrekMasters.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly AnalyticsService _analyticsService;

        public AnalyticsController(AnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var viewModel = await _analyticsService.GetDashboardDataAsync();
            return View(viewModel);
        }
    }

}
