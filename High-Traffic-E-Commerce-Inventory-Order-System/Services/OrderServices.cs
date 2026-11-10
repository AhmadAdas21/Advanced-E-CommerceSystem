using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.models.customer_model;
using High_Traffic_E_Commerce_Inventory_Order_System.models.OrderModel;
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
            if (orders == null)
            {
                throw new Exception("there is no orders  ");
            }

            var ordersResult = orders.Select(order => new GetOrders
            {
                OrderId = order.OrderId,
                CustomerName = order.Customer?.CustomerName, 
                TotalAmount = order.TotalAmount,
            }).ToList();

            return ordersResult;
        }
        public async Task<GetOrders>SearchOrder(int OrderId,CancellationToken cancellationToken)
        {
            var order =await _context.Orders.FirstOrDefaultAsync(order=>order.OrderId== OrderId);
            if (order == null)
            {
                throw new Exception($"there is no order with this id {OrderId}");
            }
            var orderResult = new GetOrders
            {
                OrderId = order.OrderId,
                CustomerName = order.Customer?.CustomerName,
                TotalAmount = order.TotalAmount,
            };
            return orderResult;
        }
        public async Task CreateOrder(CreateOrder order, CancellationToken cancellationToken)
        {
            if (order == null || order.Items == null)
            {
                throw new ArgumentException("invalid order request");
            }
            bool customerExist=await _context.Customers.AnyAsync(customer=>customer.CustomerId==order.CustomerId);
            if (!customerExist)
            {
                throw new KeyNotFoundException($"There's no customer with this id{order.CustomerId}");

            }
            if (order.Items.Any(i =>i == null ||string.IsNullOrWhiteSpace(i.SKU) ||i.Quantity <= 0))
            {
                throw new ArgumentException("Invalid order item");
            }
            var duplicatedSKU = order.Items.GroupBy(i => i.SKU, StringComparer.OrdinalIgnoreCase).Any(g => g.Count() > 1);
            if (duplicatedSKU)
            {
                throw new Exception("theres dublicated SKU in some products in the order");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            var newOrder = new Order
            {
                CustomerId = order.CustomerId,
                Items = new List<OrderItem>(),
                TotalAmount = 0

            };




        }
        public async Task CancelOrder(int OrderId, CancelOrder cancelOrder, CancellationToken cancellationToken)
        {
            if (cancelOrder == null)
            {
                throw new Exception("the cancelation form is null");
            }
            if (cancelOrder.OrderId != OrderId)
            {
                throw new Exception("the OrderId didint match");
            }

            var order = await _context.Orders.FirstOrDefaultAsync(order => order.OrderId == OrderId);
            if (order == null)
            {
                throw new Exception($"there is no order with this Id value : {OrderId}");
            }
            if (order.status == 1)
            {
                throw new Exception("the order is already cancelled");
            }
            order.status = 1;
            await _context.SaveChangesAsync();
        }
    }
}
