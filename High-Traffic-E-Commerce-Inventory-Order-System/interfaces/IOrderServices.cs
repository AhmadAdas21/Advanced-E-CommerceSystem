namespace High_Traffic_E_Commerce_Inventory_Order_System.interfaces
{
    public interface IOrderServices
    {
        public Task CreateOrder(,CancellationToken cancellationToken);
    }
}
