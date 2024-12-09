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
		[Required(ErrorMessage = "Password is mandatory")]
		[DataType(DataType.Password)]
		[RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
			ErrorMessage = "Password must be at least 8 characters long, contain one uppercase letter, one number, and one special character.")]
		public string NewPassword { get; set; }
		[Required(ErrorMessage = "ConfirmPassword is mandatory")]
		[DataType(DataType.Password)]
		[RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
	 ErrorMessage = "Password must be at least 8 characters long, contain one uppercase letter, one number, and one special character.")]
		[Compare("NewPassword", ErrorMessage = "Password and ConfirmationPassword do not match.")]
        public string ConfirmPassword { get; set; }



    }
}

