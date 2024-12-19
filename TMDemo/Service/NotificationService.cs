using Microsoft.AspNetCore.SignalR;
using TrekMasters.Hubs;

namespace TrekMasters.Service
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(AppDbContext context,INotificationRepository notificationRepository,IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
        }

        public async Task CreateNotificationAsync(string senderId, string recipientId, string message)
        {
            var notification = new Notification
            {
                SenderId = senderId,
                RecipientId = recipientId,
                Message = message
            };

           
            await _notificationRepository.AddAsync<Notification>(notification);
            await _hubContext.Clients.User(recipientId).SendAsync("ReceiveNotification", message);

        }

        public async Task<List<Notification>> GetNotificationsForUserAsync(string userId)
        {
            
            return await _notificationRepository.GetNotificationsForUserAsync(userId);
        }
        public async Task<List<Notification>> GetUnreadNotificationsForUserAsync(string userId)
        {
            return await _notificationRepository.GetUnreadNotificationsForUserAsync(userId);
        }
        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = _notificationRepository.GetNotificationById(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _notificationRepository.UpdateAsync<Notification>(notification);
            }
        }
    }
}
