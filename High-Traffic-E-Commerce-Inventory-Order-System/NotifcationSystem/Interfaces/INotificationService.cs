namespace High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem.Interfaces
{
    public interface INotificationService
    {
        Task SendAsync(NotificationMessage message,CancellationToken cancellationToken);
    }
}
