namespace TrekMasters.Repository
{
    public interface IBookingRepository:IRepository
    {
        Trek GetTrekById(int trekId);
        Booking GetBookingById(int bookingId);
        List<DateTime> GetAvailableDates(int trekId);
        Task<List<Booking>> GetOverlappingBookingsAsync(string email, DateTime startDate, DateTime endDate);
        Task<List<Booking>> GetTotalBookingsAsync();

    }

}