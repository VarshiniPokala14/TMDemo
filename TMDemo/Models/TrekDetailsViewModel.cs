namespace TMDemo.Models
{
    public class TrekDetailsViewModel
    {
        public Trek Trek { get; set; }
        public List<Booking> Bookings { get; set; }
        public List<TrekReview> Reviews { get; set; }
        public List<MonthAvailability> AvailabilityDates { get; set; }
        public List<string> TrekPlan { get; set; }
        public bool IsTrekCompleted { get; set; }
        public bool HasReviewed {  get; set; }
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
        public int RemainingSlots { get; set; } 
    }

    public class ActivityWithDate
    {
        public string Day { get; set; }  // day1, day2, day3, etc.
        public string Description { get; set; }
    }

}
