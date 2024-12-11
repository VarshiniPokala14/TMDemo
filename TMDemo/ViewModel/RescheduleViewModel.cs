namespace TrekMasters.ViewModel
{

    public class RescheduleViewModel
    {
        public int BookingId { get; set; }
        public DateTime OldStartDate { get; set; }

        [Required]
        public DateTime NewStartDate { get; set; }

        [Required]
        public string Reason { get; set; }

        public decimal ExtraAmount { get; set; }

        public List<DateTime> AvailableDates { get; set; } = new List<DateTime>();
    }




}
