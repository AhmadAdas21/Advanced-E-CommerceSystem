using High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem.Interfaces;

namespace High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem.CommunicationChanels
{
    public class EmailNotificationChanel : INotificationSystem
    {
        private readonly ILogger<EmailNotificationChanel> logger;

        public EmailNotificationChanel(ILogger<EmailNotificationChanel> logger)
        {
            this.logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.Email;

        public Task SendNotification(NotificationMessage message, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();


            logger.LogInformation("email notification simulated. Channel: {Channel}",Channel);

            return Task.CompletedTask;
        }
    }
}
