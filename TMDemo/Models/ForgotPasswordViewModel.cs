using System.ComponentModel.DataAnnotations;

namespace TMDemo.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
