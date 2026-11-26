using High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem.Interfaces;

namespace High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem
{
    public class NotificationService : INotificationService
    {
        private readonly IReadOnlyDictionary<NotificationChannel, INotificationSystem> _providers;

        public NotificationService(IEnumerable<INotificationSystem> providers)
        {
            _providers = providers.ToDictionary(p => p.Channel);
        }



        public Task SendAsync(NotificationMessage message,CancellationToken cancellationToken)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(message.Recipient))
            {
                throw new ArgumentException("notification recipient is required.");
            }

            if (!_providers.TryGetValue(message.Channel, out var provider))
            {
                throw new NotSupportedException($"notification channel {message.Channel} is not supported.");
            }

            return provider.SendNotification(message, cancellationToken);
        }
    }
    
    
}
