using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDemo.Data;
using TMDemo.Models;
using TMDemo.ViewModel;

namespace TMDemo.Controllers
{
    [Authorize] 
    public class ProfileController : Controller
    {
        private readonly UserManager<UserDetail> _userManager;
        private readonly AppDbContext _context;
        public ProfileController(UserManager<UserDetail> userManager,AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); 
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            

            return View(user);
        }

        
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var emergencyContact = await _context.EmergencyContacts
                                .FirstOrDefaultAsync(e => e.UserId == user.Id);

            

            return View(new ProfileViewModel { UserDetail = user, EmergencyContact = emergencyContact ?? new EmergencyContact()});
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            user.FirstName = model.UserDetail.FirstName;
            user.LastName = model.UserDetail.LastName;
            user.PhoneNumber = model.UserDetail.PhoneNumber;
            user.DOB = model.UserDetail.DOB;
            user.Gender = model.UserDetail.Gender;
            user.Country = model.UserDetail.Country;
            user.City = model.UserDetail.City;
            user.State = model.UserDetail.State;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }



            var emergencyContact = await _context.EmergencyContacts
            .FirstOrDefaultAsync(e => e.UserId == user.Id);

            if (emergencyContact == null)
            {
                emergencyContact = new EmergencyContact
                {
                    UserId = user.Id,
                    ContactName = model.EmergencyContact.ContactName,
                    ContactNumber = model.EmergencyContact.ContactNumber,
                    Email = model.EmergencyContact.Email,
                    Relation = model.EmergencyContact.Relation
                };
                _context.EmergencyContacts.Add(emergencyContact);
            }
            else
            {
                
                emergencyContact.ContactName = model.EmergencyContact.ContactName;
                emergencyContact.ContactNumber = model.EmergencyContact.ContactNumber;
                emergencyContact.Email = model.EmergencyContact.Email;
                emergencyContact.Relation = model.EmergencyContact.Relation;
                _context.EmergencyContacts.Update(emergencyContact);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public IActionResult MyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            
            var bookings = _context.Bookings
                .Include(b => b.Trek)
                .Where(b => b.UserId == userId && (b.IsCancelled == false || b.IsCancelled == null))
                .ToList();

            return View(bookings);
        }


    }
}

