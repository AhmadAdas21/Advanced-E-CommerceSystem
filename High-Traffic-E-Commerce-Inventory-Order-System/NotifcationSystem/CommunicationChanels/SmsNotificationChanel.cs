using High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem.Interfaces;
using Microsoft.Extensions.Logging;

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
            _logger.LogInformation("sms notification proccessed. channel: {Channel}",Channel);
            return Task.CompletedTask;
        }
    }
}
