namespace TrekMasters.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email field is required")]
        [EmailAddress(ErrorMessage ="Enter valid Email")]
        public string Email { get; set; }
    }
}
