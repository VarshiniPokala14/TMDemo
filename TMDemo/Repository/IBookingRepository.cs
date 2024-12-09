namespace TMDemo.Repository
{
    public interface IBookingRepository
    {
        Trek GetTrekById(int trekId);
        Booking GetBookingById(int bookingId);
        void AddBooking(Booking booking);
        void UpdateBooking(Booking booking);
        List<DateTime> GetAvailableDates(int trekId);
        

    }

}