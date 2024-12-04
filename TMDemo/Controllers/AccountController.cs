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
                        TempData["Message"] = "A confirmation link has been sent to your email. Please confirm your email to proceed.";
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
                    
                    return RedirectToAction("ForgotPasswordConfirmation");
                }
                
                string otp = GenerateOtp();
                DateTime otpExpiry = DateTime.UtcNow.AddMinutes(10); 
                
                HttpContext.Session.SetString("OTP", otp);
                HttpContext.Session.SetString("OTPExpiry", otpExpiry.ToString());
                HttpContext.Session.SetString("UserEmail", user.Email);
                
                await _emailSender.SendEmailAsync(model.Email, "Your OTP for Password Reset", $"Your OTP is {otp}. It will expire in 10 minutes.");
                return RedirectToAction("VerifyOtp");
            }
            return View(model);
        }
        
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
                string storedOtp = HttpContext.Session.GetString("OTP");
                DateTime otpExpiry = DateTime.Parse(HttpContext.Session.GetString("OTPExpiry"));
                string userEmail = HttpContext.Session.GetString("UserEmail");
                UserDetail user = await _userManager.FindByEmailAsync(userEmail);
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
               
                if (storedOtp == model.Otp && DateTime.UtcNow <= otpExpiry)
                {
                    return RedirectToAction("ResetPassword", new { email = userEmail,Otp=resetToken });
                }
                else
                {
                    return View(model);
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult ResetPassword(string email,string Otp)
        {
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
                    return RedirectToAction("ResetPasswordConfirmation");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); 
            return RedirectToAction("Index", "Home"); 
        }
        
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        private string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}


