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
    }
    public class TrekPopularity
    {
        public string TrekName { get; set; }
        public int BookingCount { get; set; }
    }
}
