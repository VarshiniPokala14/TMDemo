namespace TMDemo.ViewModel
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Enter valid Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Otp { get; set; }
        [Required(ErrorMessage = "Enter the Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "ReEnter The Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password and ConfirmationPassword do not match.")]
        public string ConfirmPassword { get; set; }



    }
}

