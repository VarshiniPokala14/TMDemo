namespace TrekMasters.Models
{
    public class TrekParticipant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Phone]
        public string ContactNumber { get; set; }

        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }
    }
}
