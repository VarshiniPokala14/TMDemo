namespace TMDemo.Models
{
    public class TrekDetailsViewModel
    {
        public Trek Trek { get; set; }
        public IEnumerable<TrekReview> Reviews { get; set; }
        public List<MonthAvailability> AvailabilityDates { get; set; }
        public string CurrentUserId { get; set; }
    }

    public class MonthAvailability
    {
        public string Month { get; set; }
        public List<DateRange> Dates { get; set; }
    }

    public class DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }


}
