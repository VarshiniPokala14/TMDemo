namespace TMDemo.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email field is Required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter the password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
