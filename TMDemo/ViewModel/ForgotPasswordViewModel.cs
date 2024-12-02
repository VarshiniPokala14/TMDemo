using System.ComponentModel.DataAnnotations;

namespace TMDemo.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
