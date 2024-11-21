namespace TMDemo.Models
{
    public class TrekViewModel
    {
        public Trek Trek { get; set; }
        public List<AvailabilityDate> AvailabilityDates { get; set; }
    }



    public class AvailabilityDate
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
