using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace TrekMasters.Controllers
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
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserDetail user = new UserDetail
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
                

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user,"User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                ViewData["ErrorMessage"] = "You're Registration failed";

            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserDetail user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (!user.EmailConfirmed)
                    {
                        var confirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmailDuringLogin", "Account",
                            new { userId = user.Id, token = confirmationCode }, Request.Scheme);
                        await _emailSender.SendEmailAsync(user.Email, "Email Authentication",
                            $"Please confirm your email by clicking on the link: <a href='{confirmationLink}'>Confirm Email</a>");
                        ViewData["Message"] = "A confirmation link has been sent to your email. Please confirm your email to proceed.";
                        return RedirectToAction("Login");
                    }
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password,model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {

                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains("Admin"))
                        {
                            
                            return RedirectToAction("Treks", "Admin");
                        }
                        else if (roles.Contains("User"))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "User role not assigned.");
                            return View(model);
                        }
                    }
                    
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
                ViewData["ErrorMessage"] = "Invalid Email or Password";

            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmailDuringLogin(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid email confirmation request.");
            }

            UserDetail user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }
            
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserDetail user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    ViewData["ErrorMessage"] = "Email you entered is not available or not a valid userEmail";
                    return View(model);
                }
                
                string otp = GenerateOtp();
                DateTime otpExpiry = DateTime.UtcNow.AddMinutes(10); 
                
                HttpContext.Session.SetString("OTP", otp);
                HttpContext.Session.SetString("OTPExpiry", otpExpiry.ToString());
                HttpContext.Session.SetString("UserEmail", user.Email);
                
                await _emailSender.SendEmailAsync(model.Email, "Your OTP for Password Reset", $"Your OTP is {otp}. It will expire in 10 minutes.");
				TempData["Message"] = "An OTP has been sent to your email.";
				return RedirectToAction("VerifyOtp",new {Email=user.Email});
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendOtp(string email)
        {
            
            if (!string.IsNullOrEmpty(email))
            {
                string generatedOtp = GenerateOtp();
                DateTime otpExpiry = DateTime.UtcNow.AddMinutes(10);

                HttpContext.Session.SetString("OTP", generatedOtp);
                HttpContext.Session.SetString("OTPExpiry", otpExpiry.ToString());
                HttpContext.Session.SetString("UserEmail", email);

                await _emailSender.SendEmailAsync(email, "Your OTP", $"Your OTP is {generatedOtp}");

                TempData["Message"] = "An OTP has been sent to your email.";

                return RedirectToAction("VerifyOtp",new VerifyOtpViewModel { Email=email});
            }

            TempData["ErrorMessage"] = "Unable to resend OTP. Please try again.";
            return RedirectToAction("ForgotPassword");
        }

        [HttpGet]
        public IActionResult VerifyOtp(string email)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new VerifyOtpViewModel { Email=email});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (ModelState.IsValid)
            { 
                string storedOtp = HttpContext.Session.GetString("OTP");
                DateTime otpExpiry = DateTime.Parse(HttpContext.Session.GetString("OTPExpiry"));
                string userEmail = HttpContext.Session.GetString("UserEmail");
                UserDetail user = await _userManager.FindByEmailAsync(userEmail);
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
               
                if (storedOtp == model.Otp && DateTime.UtcNow <= otpExpiry)
                {
                    return RedirectToAction("ResetPassword",new ResetPasswordViewModel { Email=userEmail,Otp=resetToken});
                }
				
				ViewData["ErrorMessage"] = "Invalid OTP";
                

            }
            return View(model);
        }
        [HttpGet]
        public IActionResult ResetPassword(string email,string Otp)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new ResetPasswordViewModel { Email = email ,Otp=Otp});
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserDetail user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ViewData["ErrorMessage"] = "Invalid User";
                    return View(model);
                }
                var result = await _userManager.ResetPasswordAsync(user, model.Otp, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Login");
                }

            }
            return View(model);
        }

        
        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            
            await _signInManager.SignOutAsync();

            
            return Redirect(returnUrl);
        }


        private string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}


