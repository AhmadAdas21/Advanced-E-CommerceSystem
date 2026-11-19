using High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem.Interfaces;

namespace High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem.CommunicationChanels
{
    public class PushNotificationChanel:INotificationSystem
    {
        private readonly ILogger<PushNotificationChanel> logger;
        public PushNotificationChanel(ILogger<PushNotificationChanel> logger)
        {
            this.logger = logger;
        }
        public NotificationChannel chanel=NotificationChannel.Push;
        public Task SendNotification(NotificationMessage notificationMessage, CancellationToken cancellationToken)
        {
            string logmessage = "";
        }
    }
}
