namespace TrekMasters.Service
{
    public interface IBookingService
    {

        Task<bool> ProcessPayment(int bookingId, string paymentMethod);

        CancellationViewModel GetCancellationViewModel(int bookingId);
        void ProcessCancellation(CancellationViewModel model);
        RescheduleViewModel GetRescheduleViewModel(int bookingId);
        void ProcessReschedule(RescheduleViewModel model);

        Task<Booking> CreateBookingAsync(AddUsersViewModel model, string userId);
        void AddParticipant(int bookingId, ParticipantViewModel participant);
        AddUsersViewModel GetAddUsersViewModel(int trekId ,string startDate, string userEmail);
    }

}
