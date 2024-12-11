using TrekMasters.Service;

namespace TrekMasters.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IActionResult> BySeason() => View(await _categoryService.GetTreksGroupedBySeasonAsync());

        public async Task<IActionResult> ByDifficulty() => View(await _categoryService.GetTreksGroupedByDifficultyAsync());

        public async Task<IActionResult> ByRegion() => View(await _categoryService.GetTreksGroupedByRegionAsync());

        public async Task<IActionResult> ByDuration() => View(await _categoryService.GetTreksGroupedByDurationAsync());

        public async Task<IActionResult> ByMonth() => View(await _categoryService.GetTreksGroupedByMonthAsync());

        [HttpGet]
        public async Task<IActionResult> FilterByCategory(string categoryType, string filterValue)
        {
            var model = await _categoryService.FilterTreksByCategoryAsync(categoryType, filterValue);
            return View("FilteredView", model);
        }
    }
}