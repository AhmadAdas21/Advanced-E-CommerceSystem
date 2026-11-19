using High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem.Interfaces;

namespace High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem.CommunicationChanels
{
    public class PushNotificationChanel : INotificationSystem
    {
        private readonly ILogger<PushNotificationChanel> logger;

        public PushNotificationChanel(ILogger<PushNotificationChanel> logger)
        {
            this.logger = logger;
        }

        
        public NotificationChannel Channel => NotificationChannel.Push;

        public Task SendNotification(NotificationMessage notificationMessage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            logger.LogInformation("Push notification simulated. Channel: {Channel}",Channel);
            return Task.CompletedTask;
        }
    }
}
