using High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem.Interfaces;

namespace High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem.CommunicationChanels
{
    public class SmsNotificationChanel: INotificationSystem
    {
        private readonly ILogger<SmsNotificationChanel> _logger;
        public SmsNotificationChanel(ILogger<SmsNotificationChanel> logger)
        {
            _logger = logger;
        }
        public NotificationChannel Channel => NotificationChannel.SMS;
        public Task SendNotification(NotificationMessage notificationMessage,CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string logMessage = $"sms notification sent to {notificationMessage.Recipient} with this content {notificationMessage.Subject}";
            _logger.LogInformation(logMessage);
            return Task.CompletedTask;
        }
    }
}
