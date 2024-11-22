namespace TMDemo.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace TMDemo.Models
    {
        public class Payment
        {
            [Key]
            public int PaymentId { get; set; }

            [Required]
            public int BookingId { get; set; }

            [Required]
            [Column(TypeName = "decimal(10, 2)")]
            public decimal Amount { get; set; }

            [Required]
            public DateTime PaymentDate { get; set; }

            [Required]
            public string PaymentMethod { get; set; } // Enum: Credit Card, Debit Card, UPI

            // Navigation property
            public Booking Booking { get; set; }
        }
    }

}
