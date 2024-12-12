namespace TrekMasters.ViewModel
{
    //public class AddUsersViewModel
    //{
    //    public int TrekId { get; set; }
    //    public string TrekName { get; set; }
    //    public DateTime StartDate { get; set; }
    //    public List<string> Emails { get; set; }
    //}
    public class AddUsersViewModel
    {
        public int TrekId { get; set; }
        public string TrekName { get; set; }
        public DateTime StartDate { get; set; }
        public List<ParticipantViewModel> Participants { get; set; } = new List<ParticipantViewModel>();
    }

    public class ParticipantViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
    }

}
