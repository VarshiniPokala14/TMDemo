﻿using Microsoft.EntityFrameworkCore;
using TMDemo.Service;

public class TrekController : Controller
{
    private readonly ITrekService _trekService;
    
    public TrekController(ITrekService trekService)
    {
        _trekService = trekService;
    }

    public async Task<IActionResult> AllTreks()
    {
        var treks = await _trekService.GetAllTreksAsync();
        return View(treks);
    }
    public async Task<IActionResult> GetImage(int trekId)
    {
        var image = await _trekService.GetTrekImageAsync(trekId);
        if (image != null)
        {
            return File(image, "image/jpg");
        }
        return NotFound();
    }
    public async Task<IActionResult> UpcomingTreks()
    {
        var treks = await _trekService.GetUpcomingTreksAsync();
        return View(treks);
    }
    public async Task<IActionResult> Details(int trekId)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var viewModel = await _trekService.GetTrekDetailsAsync(trekId, userId);

        if (viewModel == null) return NotFound();

        return View(viewModel);
    }
    [HttpPost]
    public async Task<IActionResult> AddReview(int trekId, string reviewText)
    {
        var errorMessage = await _trekService.AddReviewAsync(trekId, reviewText);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            TempData["ReviewMessage"] = errorMessage;
        }

        return RedirectToAction("Details", new { trekId = trekId });
    }
    public async Task<IActionResult> Index(string searchString)
    {
        var treks = await _trekService.SearchTreksAsync(searchString);
        if (treks == null || !treks.Any())
        {
            RedirectToAction("AllTreks");
        }

        return View(treks);
    }
    [Authorize]
    [HttpPost]
    
    public async Task<IActionResult> NotifyMe(int trekId)
    {
        string userEmail = User.FindFirstValue(ClaimTypes.Email);

        // Check if the user has already requested notifications for this trek
        var existingRequest = await _trekService.GetNotificationRequestAsync(trekId, userEmail);

        if (existingRequest == null)
        {
            // Add a new notification request
            await _trekService.AddNotificationRequestAsync(trekId, userEmail);
            TempData["NotificationMessage"] = "You will be notified when availability is added for this trek.";
        }
        else
        {
            TempData["NotificationMessage"] = "You are already subscribed for notifications for this trek.";
        }

        return RedirectToAction("Details", new { trekId = trekId });
    }




}
