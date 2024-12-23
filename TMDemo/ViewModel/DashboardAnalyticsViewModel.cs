using TrekMasters.Models;

namespace TrekMasters.ViewModel
{
    public class DashboardAnalyticsViewModel
    {
        public int TotalBookings { get; set; }
        public int TotalUsers { get; set; }
        public int CompletedTreks { get; set; }
        public int OngoingTreks { get; set; }
        public decimal? TotalRevenue {  get; set; }
        public int Cancellations { get; set; }
        public int TotalTreks { get; set; }
        public int UpcomingTreks { get; set; }
        public Dictionary<string, int> BookingsByRegion { get; set; }
        public List<TrekPopularity> TopTreks { get; set; }
        public List<TrekPopularity> AllTreks { get; set; }
        public List<UserDetails> UserDetails { get; set; }
        public List<Trek> Treks { get; set; }
        public List<TrekBookingViewModel> OngoingTrekDetails { get; set; }


    }
    public class TrekBookingViewModel
    {
        public string TrekName { get; set; }
        public string TrekRegion { get; set; }
        public int TrekDurationDays { get; set; }
        public List<UserListViewModel> BookedUsers { get; set; }
    }



    public class UserListViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int NumberOfPeople { get; set; }
        public decimal TotalAmount { get; set; }
        public int BookingId {  get; set; }
        public DateTime BookingDate { get; set; }
    }

    public class TrekPopularity
    {
        public string TrekName { get; set; }
        public int BookingCount { get; set; }
    }
    public class UserDetails
    {
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
    }

}
