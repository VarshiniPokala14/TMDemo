namespace TrekMasters.ViewModel
{
    public class TrekCancellationViewModel
    {
        public int AvailabilityId { get; set; }
        public string TrekName { get; set; }
        public DateTime StartDate { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRefundAmount { get; set; }
        public string Reason { get; set; }
    }

}
