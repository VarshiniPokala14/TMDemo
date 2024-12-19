using Microsoft.AspNetCore.SignalR;

namespace TrekMasters.Hubs
{

    public class NotificationHub : Hub
    {
        private readonly INotificationService _notificationService;
        public NotificationHub(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        //public async Task SendNotification(string userId, string message)
        //{
        //    // Notify a specific user
        //    await Clients.User(userId).SendAsync("ReceiveNotificationUpdate", message);
        //}
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            Console.WriteLine($"Connected user ID: {userId}");

            if (userId != null)
            {
                var unreadnotifications = await _notificationService.GetUnreadNotificationsForUserAsync(userId);

                foreach (var notification in unreadnotifications)
                {
                    await Clients.Caller.SendAsync("ReceiveNotification", notification.Message);
                }

                // Mark notifications as read
                var notificationIds = unreadnotifications.Select(n => n.NotificationId).ToList();
                

            }

            await base.OnConnectedAsync();
        }
    }


}
