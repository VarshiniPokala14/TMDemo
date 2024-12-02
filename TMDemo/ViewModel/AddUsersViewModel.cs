namespace TMDemo.ViewModel
{
    public class AddUsersViewModel
    {
        public int TrekId { get; set; }
        public string TrekName { get; set; }
        public DateTime StartDate { get; set; }
        public List<string> Emails { get; set; }
    }

    public class AddedUser
    {
        public string FirstName { get; set; }
        public string Email { get; set; }
    }

}
