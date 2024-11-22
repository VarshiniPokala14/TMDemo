using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMDemo.Models
{
    public class Cancellation
    {
        [Key]
        public int CancellationId { get; set; }

        [Required]
        [ForeignKey("Booking")]
        public int BookingId { get; set; } // Foreign key to Booking

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CancellationDate { get; set; }

        public string Reason { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal RefundAmount { get; set; }

        // Navigation property
        public Booking? Booking { get; set; }
    }
    public class CancellationViewModel
    {
        public int BookingId { get; set; }
        public string Reason { get; set; }
        public decimal RefundAmount { get; set; }
        public Booking? Booking { get; set; }
    }

}
