namespace TrekMasters.ViewModel
{
    public class TrekAvailabilityViewModel
    {
        public int AvailabilityId { get; set; }
        public DateTime StartDate { get; set; }
        public int MaxGroupSize { get; set; }
        public int RemainingSlots { get; set; }
    }
    public class UserViewModel
    {
        public string UserId {  get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int NumberOfPeople { get; set; }
        public int BookingId {  get; set; }
        public DateTime BookingDate { get; set; }
    }
    public class TrekParticipantViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
    }


}
