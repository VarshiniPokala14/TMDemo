//using Microsoft.AspNetCore.SignalR;
//using TrekMasters.Models;

//namespace TrekMasters.Hubs
//{
//    //public class NotificationHub : Hub
//    //{
//    //       //public async Task SendNotificationToAdmin(string message)
//    //       //{
//    //       //	string userEmail = Context.User?.Identity?.Name;  // You can get user email like this
//    //       //	if (userEmail != null)
//    //       //	{
//    //       //		await Clients.User(userEmail).SendAsync("ReceiveNotification", message);
//    //       //	}
//    //       //}

//    //       //public async Task SendNotificationToUser(string userEmail, string message)
//    //       //{
//    //       //	await Clients.User(userEmail).SendAsync("ReceiveNotification", message);
//    //       //}
//    //       private static List<string> Notifications = new List<string>();

//    //       public async Task SendNotification(string message)
//    //       {
//    //           // Store the notification
//    //           Notifications.Add(message);

//    //           string userEmail = Context.User?.Identity?.Name;  // You can get user email like this
//    //           if (userEmail != null)
//    //           {
//    //               await Clients.User(userEmail).SendAsync("ReceiveNotification", message);
//    //           }
//    //       }

//    //       public override Task OnConnectedAsync()
//    //       {
//    //           // Send all stored notifications to the newly connected client
//    //           Clients.Caller.SendAsync("LoadNotifications", Notifications);
//    //           return base.OnConnectedAsync();
//    //       }

//    //       public Task RemoveNotification(string message)
//    //       {
//    //           Notifications.Remove(message); // Remove notification from storage
//    //           return Task.CompletedTask;
//    //       }
//    //   }
//    public class NotificationHub : Hub
//    {
//        private readonly INotificationRepository _notificationRepository;

//        public NotificationHub(INotificationRepository notificationRepository)
//        {
//            _notificationRepository = notificationRepository;
//        }

//        public override async Task OnConnectedAsync()
//        {
//            var userId = Context.User?.Identity?.Name;
//            if (!string.IsNullOrEmpty(userId))
//            {
//                // Send unread notifications to the connected user
//                var notifications = await _notificationRepository.GetUserNotificationsAsync(userId);
//                await Clients.Caller.SendAsync("LoadNotifications", notifications);
//            }

//            await base.OnConnectedAsync();
//        }

//        public async Task RemoveNotification(int notificationId)
//        {
//            await _notificationRepository.DeleteNotificationAsync(notificationId);
//            await Clients.Caller.SendAsync("NotificationRemoved", notificationId);
//        }
//    }


//}
