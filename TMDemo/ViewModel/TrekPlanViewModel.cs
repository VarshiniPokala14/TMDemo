namespace TMDemo.ViewModel
{
    public class ActivityInputModel
    {
        public int Day { get; set; }
        public string ActivityDescription { get; set; }
    }

    public class TrekPlanViewModel
    {
        public int TrekId { get; set; }
        public int DurationDays { get; set; }
        public List<ActivityInputModel> Activities { get; set; }
    }

}
