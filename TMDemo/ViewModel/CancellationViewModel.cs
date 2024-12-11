
namespace TrekMasters.ViewModel
{
    public class CancellationViewModel
    {
        public int BookingId { get; set; }
        public string Reason { get; set; }
        public decimal RefundAmount { get; set; }
        public Booking? Booking { get; set; }
    }

}
