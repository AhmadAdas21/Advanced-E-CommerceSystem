using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.models.customer_model;
using Microsoft.EntityFrameworkCore;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly DataBaseContext _context;

        public OrderServices(DataBaseContext context)
        {
            _context = context; 
        }

        public async Task<List<GetOrders>> GetOrders(CancellationToken cancellationToken)
        {
            var orders = await _context.Orders.Include(order => order.Customer) .ToListAsync(cancellationToken);

            var OrdersResult = orders.Select(order => new GetOrders
            {
                OrderId = order.OrderId,
                CustomerName = order.Customer?.CustomerName, 
                TotalAmount = order.TotalAmount,
            }).ToList();

            return OrdersResult;
        }
    }
}
