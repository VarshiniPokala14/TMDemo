using Microsoft.AspNetCore.Identity;

namespace TMDemo.Models
{
    public class UserDetail : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public EmergencyContact? EmergencyContact { get; set; }
        public ICollection<TrekReview>? TrekReviews { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}
