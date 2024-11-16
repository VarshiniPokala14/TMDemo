using System.ComponentModel.DataAnnotations;

namespace TMDemo.Models
{
    public class VerifyOtpViewModel
    {
        [Required]
        public string Otp { get; set; }
        

    }
}

