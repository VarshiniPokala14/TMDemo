using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TMDemo.Models.TMDemo.Models;

namespace TMDemo.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        [ForeignKey("UserDetail")]
        [Required]
        public string UserId { get; set; } // Foreign key to UserDetail
        [ForeignKey("Trek")]
        [Required]
        public int TrekId { get; set; } // Foreign key to Trek

        [Required]
        [DataType(DataType.DateTime)]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime BookingDate { get; set; }  

        [Required]
        public int NumberOfPeople { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime TrekStartDate { get; set; }
        public bool? PaymentSuccess {  get; set; }
        
        public bool? IsCancelled { get; set; }

        [DataType(DataType.DateTime)]

        public DateTime? CancellationDate { get; set; } = null;

        public string? Reason { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? RefundAmount { get; set; }
        
        public string? RescheduleReason { get; set; }

        public decimal? ExtraAmount { get; set; } // Calculated extra charge

        // Navigation properties
        public UserDetail? User { get; set; }
        public Trek? Trek { get; set; }
       

    }
}
