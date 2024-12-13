namespace TrekMasters.Service
{
    public interface IBookingService
    {

        Task<bool> ProcessPayment(int bookingId, string paymentMethod);

        CancellationViewModel GetCancellationViewModel(int bookingId);
        Task ProcessCancellation(CancellationViewModel model);
        RescheduleViewModel GetRescheduleViewModel(int bookingId);
        Task ProcessReschedule(RescheduleViewModel model);

        Task<(Booking, List<string>)> CreateBookingAsync(AddUsersViewModel model, string userId);
        Task AddParticipant(int bookingId, ParticipantViewModel participant);
        AddUsersViewModel GetAddUsersViewModel(int trekId ,string startDate, string userEmail);
    }

}
