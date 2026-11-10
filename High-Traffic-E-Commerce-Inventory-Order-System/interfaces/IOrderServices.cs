using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s;

namespace High_Traffic_E_Commerce_Inventory_Order_System.interfaces
{
    public interface IOrderServices
    {
        public Task CreateOrder(CreateOrder Order,CancellationToken cancellationToken);
        public Task UpdateOrder(int OrderId,UpdateOrder Order,CancellationToken cancellationToken);
        public Task CancelOrder(int OrderId,CancelOrder Order,CancellationToken cancellationToken);
        public Task<List<GetOrders>> GetOrders(CancellationToken cancellationToken);
        public Task<GetOrders>SearchOrder(int OrderId,CancellationToken cancellationToken);

    }
}
