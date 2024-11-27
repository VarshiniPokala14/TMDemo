using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMDemo.Models
{
    public class CancellationViewModel
    {
        public int BookingId { get; set; }
        public string Reason { get; set; }
        public decimal RefundAmount { get; set; }
        public Booking? Booking { get; set; }
    }

}
