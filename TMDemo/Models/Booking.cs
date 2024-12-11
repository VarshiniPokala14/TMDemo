

namespace TrekMasters.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        [ForeignKey("UserDetail")]
        [Required]
        public string UserId { get; set; } 
        [ForeignKey("Trek")]
        [Required]
        public int TrekId { get; set; } 

        [Required]
        [DataType(DataType.DateTime)]
        
        public DateTime BookingDate { get; set; }  

        [Required]
        public int NumberOfPeople { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [FutureOrTodayDate(ErrorMessage = "Date shouldn't be past date")]
        public DateTime TrekStartDate { get; set; }
        public bool? PaymentSuccess {  get; set; }
        
        public bool? IsCancelled { get; set; }

        [DataType(DataType.DateTime)]

        public DateTime? CancellationDate { get; set; } = null;

        public string? Reason { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? RefundAmount { get; set; }
        
        public string? RescheduleReason { get; set; }

        public decimal? ExtraAmount { get; set; } 
        public UserDetail? User { get; set; }
        public Trek? Trek { get; set; }
       

    }
}
