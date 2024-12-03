
namespace TMDemo.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email field is required")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
