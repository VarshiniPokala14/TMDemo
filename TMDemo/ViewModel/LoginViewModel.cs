namespace TrekMasters.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email field is Required")]
        [EmailAddress(ErrorMessage ="Enter valid Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter valid password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
