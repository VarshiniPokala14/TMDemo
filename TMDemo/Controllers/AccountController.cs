using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TMDemo.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
namespace TMDemo.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<UserDetail> _userManager;
        private readonly SignInManager<UserDetail> _signInManager;
        private readonly IUserStore<UserDetail> _userStore;
        private readonly IEmailSender _emailSender;
        public AccountController(UserManager<UserDetail> userManager, SignInManager<UserDetail> signInManager, IUserStore<UserDetail> userStore, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = userStore;
            _emailSender = emailSender;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new UserDetail
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    DOB = model.DOB,
                    Gender = model.Gender,
                    Country = model.Country,
                    City = model.City,
                    State = model.State,
                    UserName=model.FirstName
                };
                //await _userStore.SetUserNameAsync(user, model.FirstName, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password,model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        
                        return RedirectToAction("Index", "Home");
                    }
                    if (result.IsLockedOut)
                    {
                        
                        return View("Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return Ok(ModelState);
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        // Forgot Password Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Do not reveal that the email doesn't exist or is not confirmed
                    return RedirectToAction("ForgotPasswordConfirmation");
                }
                // Generate OTP
                var otp = GenerateOtp();
                var otpExpiry = DateTime.UtcNow.AddMinutes(10); // OTP expiry time
                // Save OTP in a temp store, here it's in a simple session for demonstration purposes
                HttpContext.Session.SetString("OTP", otp);
                HttpContext.Session.SetString("OTPExpiry", otpExpiry.ToString());
                HttpContext.Session.SetString("UserEmail", user.Email);
                // Send OTP to email
                await _emailSender.SendEmailAsync(model.Email, "Your OTP for Password Reset", $"Your OTP is {otp}. It will expire in 10 minutes.");
                return RedirectToAction("VerifyOtp");
            }
            return View(model);
        }
        // Verify OTP
        [HttpGet]
        public IActionResult VerifyOtp()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the OTP and expiry time from session
                var storedOtp = HttpContext.Session.GetString("OTP");
                var otpExpiry = DateTime.Parse(HttpContext.Session.GetString("OTPExpiry"));
                var userEmail = HttpContext.Session.GetString("UserEmail");
                var user = await _userManager.FindByEmailAsync(userEmail);
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                // Validate OTP
                if (storedOtp == model.Otp && DateTime.UtcNow <= otpExpiry)
                {
                    return RedirectToAction("ResetPassword", new { email = userEmail,Otp=resetToken });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid or expired OTP.");
                }
            }
            return View(model);
        }
        // Reset Password View
        [HttpGet]
        public IActionResult ResetPassword(string email,string Otp)
        {
            return View(new ResetPasswordViewModel { Email = email ,Otp=Otp});
        }
        // Reset Password Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }
                var result = await _userManager.ResetPasswordAsync(user, model.Otp, model.NewPassword);
                if (result.Succeeded)
                {
                    // After successful password reset, sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Login");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();  // Logs the user out
            return RedirectToAction("Index", "Home"); // Redirect to Home page or Login page
        }
        // Confirmation views
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        // Helper method to generate OTP (can be enhanced)
        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}


