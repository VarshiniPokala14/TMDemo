﻿using System.ComponentModel.DataAnnotations;

namespace TMDemo.ViewModel
{
    public class VerifyOtpViewModel
    {
        [Required(ErrorMessage = "OTP is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be a 6-digit number.")]
        public string Otp { get; set; }


    }
}

