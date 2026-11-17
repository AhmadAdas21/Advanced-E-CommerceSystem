namespace High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem
{
    public record NotificationMessage(string Recipient,string Subject,string Body, NotificationChannel Channel);
}
