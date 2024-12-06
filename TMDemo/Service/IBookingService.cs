namespace TMDemo.Service
{
    public interface IBookingService
    {
        AddUsersViewModel GetAddUsersViewModel(int trekId, string startDate, string userEmail);
        void AddMember(AddUsersViewModel model, string email);
        Booking CreateBooking(AddUsersViewModel model, string userId);
        Task<bool> ProcessPayment(int bookingId, string paymentMethod);

        CancellationViewModel GetCancellationViewModel(int bookingId);
        void ProcessCancellation(CancellationViewModel model);
        RescheduleViewModel GetRescheduleViewModel(int bookingId);
        void ProcessReschedule(RescheduleViewModel model);
    }

}
