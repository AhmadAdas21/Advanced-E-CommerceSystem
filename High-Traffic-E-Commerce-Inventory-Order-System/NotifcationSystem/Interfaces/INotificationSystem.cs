namespace High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem.Interfaces
{
    public interface INotificationSystem
    {
        NotificationChannel Channel { get; }

        Task SendNotification(NotificationMessage message,CancellationToken cancellationToken);
    }
}
