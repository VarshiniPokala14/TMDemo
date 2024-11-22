using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMDemo.Models
{
    public class Reschedule
    {
        [Key]
        public int RescheduleId { get; set; }

        [Required]
        [ForeignKey("Booking")]
        public int BookingId { get; set; } // Foreign key to Booking

        public DateTime? OldAvailableDate { get; set; } // Nullable to handle cases where old date may not exist
        [Required]
        public DateTime NewAvailableDate { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal ExtraAmount { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime RescheduleDate { get; set; } 

        // Navigation property
        public Booking? Booking { get; set; }
    }
}
