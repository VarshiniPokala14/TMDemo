namespace TrekMasters.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="FirstName is Required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is Required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage ="Enter valid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone Number must be exactly 10 digits.")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "DOB is Required")]
        [DataType(DataType.Date)]
        [DOBValidation(ErrorMessage = "Date shouldn't be Future date")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Gender is Required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Country is Required")]
        public string Country { get; set; }

        [Required(ErrorMessage = "City is Required")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is Required")]
        public string State { get; set; }

        [Required(ErrorMessage = "Password is mandatory")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
    ErrorMessage = "Password must be at least 8 characters long, contain one uppercase letter, one number, and one special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "ConfirmPassword is mandatory")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmationPassword do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
