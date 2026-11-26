using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.ResultPattern;

namespace High_Traffic_E_Commerce_Inventory_Order_System.interfaces
{
    public interface IOrderServices
    {
        public Task<Result<int>> CreateOrder(CreateOrder Order,CancellationToken cancellationToken);
        
        public Task<Result<int>> CancelOrder(int OrderId,CancelOrder Order,CancellationToken cancellationToken);
        public Task<Result<List<GetOrders>>> GetOrders(CancellationToken cancellationToken);
        Task<Result<GetOrders>> SearchOrder(int OrderId,CancellationToken cancellationToken);

    }
}
