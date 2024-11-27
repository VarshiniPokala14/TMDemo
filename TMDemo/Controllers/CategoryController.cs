using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDemo.Data;
using TMDemo.Models;

namespace TMDemo.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> BySeason()
        {
            var treksGroupedBySeason = await _context.Treks
                .SelectMany(trek => trek.Season, (trek, season) => new { trek, season })
                .GroupBy(ts => ts.season)
                .Select(group => new GroupedTreksByCategory
                {
                    Category = group.Key,
                    Treks = group.Select(ts => ts.trek).Distinct().ToList()
                })
                .OrderBy(group => group.Category)
                .ToListAsync();
            return View(treksGroupedBySeason);
        }
        public async Task<IActionResult> ByDifficulty()
        {
            
            var treksGroupedByDifficulty = await _context.Treks
                .GroupBy(t => t.DifficultyLevel) 
                .Select(group => new GroupedTreksByCategory
                {
                    Category = group.Key, 
                    Treks = group.ToList()
                })
                .OrderBy(group => group.Category) 
                .ToListAsync();
            return View(treksGroupedByDifficulty);
        }
        public async Task<IActionResult> ByRegion()
        {

            var treksGroupedByRegion = await _context.Treks
                .GroupBy(t => t.Region)
                .Select(group => new GroupedTreksByCategory
                {
                    Category = group.Key,
                    Treks = group.ToList()
                })
                .OrderBy(group => group.Category)
                .ToListAsync();
            return View(treksGroupedByRegion);
        }
        public async Task<IActionResult> ByDuration()
        {
            
            var treksGroupedByDuration = await _context.Treks
                .GroupBy(t => t.DurationDays)
                .Select(group => new GroupedTreksByCategory
                {
                    Category = Convert.ToString(group.Key)+" Days",
                    Treks = group.ToList()
                })
                .OrderBy(group => group.Category)
                .ToListAsync();
            return View(treksGroupedByDuration);
        }
        public async Task<IActionResult> ByMonth()
        {
            
            var availabilities = await _context.Availabilities
                .Include(a => a.Trek)
                .Where(a => a.Trek != null) 
                .ToListAsync();

            
            var groupedByMonth = availabilities
                .GroupBy(a => a.Month)
                .Where(g => g.Any()) 
                .OrderBy(g => DateTime.ParseExact(g.Key, "MMMM", null)) 
                .ToList();

            
            var model = groupedByMonth.Select(group => new GroupedTreksByCategory
            {
                Category = group.Key,
                Treks = group.Select(g=>g.Trek).Distinct().ToList()
            }).ToList();

            return View(model);
        }

    }
}
