using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using TMDemo.Data;
using TMDemo.Models;

namespace TMDemo.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult AddTrek()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTrek(AddTrekViewModel model)
        {
            if (ModelState.IsValid)
            {
                var trek = new Trek
                {
                    Name = model.Name,
                    Region = model.Region,
                    Description = model.Description,
                    DifficultyLevel = model.DifficultyLevel,
                    DurationDays = model.DurationDays,
                    HighAltitude = model.HighAltitude,
                    Price = model.Price,
                    Season = model.SelectedSeasons
                };

                if (model.TrekImgFile != null)
                {
                    using var memoryStream = new MemoryStream();
                    await model.TrekImgFile.CopyToAsync(memoryStream);
                    trek.TrekImg = memoryStream.ToArray();
                }

                
                _context.Treks.Add(trek);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index","Home"); 
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> AddAvailability()
        {
            
            ViewBag.Treks = await _context.Treks
                .OrderBy(t => t.Name)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAvailability(Availability availability)

        {
            ModelState.Remove("Month");
            var trek = await _context.Treks.FindAsync(availability.TrekId);
            if (trek == null)
            {
                ModelState.AddModelError("", "Invalid Trek selected.");
                ViewBag.Treks = await _context.Treks.OrderBy(t => t.Name).ToListAsync();
                return View(availability);
            }
            availability.Month = availability.StartDate.ToString("MMMM");
            if (string.IsNullOrWhiteSpace(availability.Month))
            {
                ModelState.AddModelError("Month", "Month is required.");
            }
            availability.EndDate = availability.StartDate.AddDays(trek.DurationDays - 1);
            if (ModelState.IsValid)
            {
                _context.Availabilities.Add(availability);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","Admin");
            }

            else
            {
                return Ok(ModelState);
            }
            ViewBag.Treks = await _context.Treks.OrderBy(t => t.Name).ToListAsync();
            return View(availability);
        }
        public async Task<IActionResult> Index()
        {
           
            var availabilities = await _context.Availabilities
                .Include(a => a.Trek)
                .OrderBy(a => a.StartDate)
                .ToListAsync();

            return View(availabilities);
        }
    }
}
