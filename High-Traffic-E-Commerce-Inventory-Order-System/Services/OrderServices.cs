using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;

using High_Traffic_E_Commerce_Inventory_Order_System.models.OrderModel;
using High_Traffic_E_Commerce_Inventory_Order_System.models.ProductsModel;
using High_Traffic_E_Commerce_Inventory_Order_System.Exceptions;
using Microsoft.EntityFrameworkCore;
using High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem.Interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.NotifcationSystem;
using System.Threading.Channels;
using High_Traffic_E_Commerce_Inventory_Order_System.ResultPattern;
using Microsoft.Data.SqlClient;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly DataBaseContext _context;
        private readonly IPricingEngine _pricingEngine;
        private readonly IPricingStrategySelector _pricingSelector;
        private readonly IDistributedCache distributedCache;
        private readonly ILogger<OrderServices> _logger;
        private readonly INotificationService notificationService;
        public OrderServices(INotificationService notificationService,DataBaseContext context,IPricingEngine pricingEngine, IPricingStrategySelector pricingSelector,IDistributedCache distributedCache,ILogger<OrderServices>logger)
        {
            _context = context;
            _pricingEngine = pricingEngine;
            _pricingSelector = pricingSelector;
            this.distributedCache = distributedCache;
            _logger = logger;
            this.notificationService = notificationService;
        }

        public async Task<Result<List<GetOrders>>> GetOrders(CancellationToken cancellationToken)
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

            return Result<List<GetOrders>>.Success(ordersResult);
        }
        public async Task<Result<GetOrders>> SearchOrder(int orderId,CancellationToken cancellationToken)
        {
            var order =await _context.Orders.AsNoTracking().Include(o => o.Customer).FirstOrDefaultAsync(order=>order.OrderId== orderId,cancellationToken);
            if (order == null)
            {
                return Result<GetOrders>.Failure(new AppError( ErrorType.NotFound,"ORDER_NOT_FOUND",$"order with this id:{orderId} is not found."
           )
       );
            }
            var orderResult = new GetOrders
            {
                OrderId = order.OrderId,
                CustomerName = order.Customer?.CustomerName,
                TotalAmount = order.TotalAmount,
            };
            return Result<GetOrders>.Success(orderResult); 
        }
        public async Task<Result<int>> CreateOrder(CreateOrder order, CancellationToken cancellationToken)
        {
            if (order == null || order.Items == null)
            {
                return Result<int>.Failure(new AppError( ErrorType.Validation,"INVALID_ORDER","Invalid order request."));
            }
            var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.CustomerId == order.CustomerId,cancellationToken);

            if (customer == null)
            {
                return Result<int>.Failure(new AppError(ErrorType.NotFound,"CUSTOMER_NOT_FOUND",$"Customer {order.CustomerId} not found."));
            }

            var strategy = _pricingSelector.SelectPricingStrategy(customer,DateTimeOffset.UtcNow);
            if (order.Items.Any(i =>i == null ||string.IsNullOrWhiteSpace(i.SKU) ||i.Quantity <= 0))
            {
                return Result<int>.Failure(new AppError(ErrorType.Validation, "INVALID_ORDER_ITEM","Invalid order item."));
            }
            var duplicatedSKU = order.Items.GroupBy(i => i.SKU, StringComparer.OrdinalIgnoreCase).Any(g => g.Count() > 1);
            if (duplicatedSKU)
            {
                return Result<int>.Failure(new AppError( ErrorType.Validation,"DUPLICATE_SKU","Duplicate SKU found in order."));
            }
            if (order.RequestId == Guid.Empty)
            {
                return Result<int>.Failure(new AppError(ErrorType.Validation,"INVALID_REQUEST_ID","RequestId is required."));
            }
            var requestExists = await _context.Orders.AsNoTracking().AnyAsync(o => o.RequestId == order.RequestId,cancellationToken);

            if (requestExists)
            {
                return Result<int>.Failure(new AppError(ErrorType.Conflict,"DUPLICATE_REQUEST", "An order with this RequestId already exists."));
            }
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            var newOrder = new Order
            {
                CustomerId = order.CustomerId,
                Items = new List<OrderItem>(),
                TotalAmount = 0,
                RequestId = order.RequestId,

            };


           

           foreach(var item in order.Items.OrderBy(x => x.SKU))
            {
                var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(product=>product.SKU==item.SKU,cancellationToken);
                if (product == null)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return Result<int>.Failure(new AppError( ErrorType.NotFound,"PRODUCT_NOT_FOUND",$"Product {item.SKU} not found."));
                }
                if (!product.IsActive)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return Result<int>.Failure(new AppError(ErrorType.Conflict,"PRODUCT_INACTIVE",$"Product {item.SKU} is inactive."));
                }

                int affectedRows = await _context.Products.Where(p =>p.ProductId == product.ProductId &&p.IsActive && p.AvailableStock >= item.Quantity)
                         .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.AvailableStock, p => p.AvailableStock - item.Quantity),  cancellationToken);

                if (affectedRows != 1)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return Result<int>.Failure(new AppError(ErrorType.Conflict, "INSUFFICIENT_STOCK",$"Insufficient stock for product {item.SKU}."));
                }
                decimal unitPrice = decimal.Round( _pricingEngine.CalculatePrice(strategy, product.ProductPrice),2,MidpointRounding.AwayFromZero);


                decimal totalPrice = unitPrice * item.Quantity;

                newOrder.Items.Add(new OrderItem
                {
                    ProductId = product.ProductId,
                    OrderQuantity = item.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice
                });

                newOrder.TotalAmount += totalPrice;



                
            }
            _context.Orders.Add(newOrder);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                await transaction.RollbackAsync(cancellationToken);

                return Result<int>.Failure(new AppError(ErrorType.Conflict, "DUPLICATE_REQUEST", "An order with this RequestId already exists."));
            }

            try
            {
                await distributedCache.RemoveAsync("products:v2:all", CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Failed to invalidate products list after order creation.");
            }

            foreach (var item in newOrder.Items)
            {
                try
                {
                    await distributedCache.RemoveAsync($"products:v2:id:{item.ProductId}", CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,"Failed to invalidate product {ProductId} after order creation.",item.ProductId);
                }
            }
           
            try
            {
                var channel = customer.CustomerPrefferdChannel;
                var recipient = channel switch
                {
                    NotificationChannel.Email => customer.CustomerEmail,
                    NotificationChannel.SMS => customer.CustomerPhone,
                    _ => throw new NotSupportedException("Notification channel is not supported.")
                };
                var notification = new NotificationMessage(
                    Recipient: recipient,
                    Subject: $"order  with id #{newOrder.OrderId} confirmed",
                    Body: $"Your order with id #{newOrder.OrderId} created successfully.",
                    Channel: channel
                );

                await notificationService.SendAsync(notification,CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "notification failed after creating order with order id  {OrderId}.", newOrder.OrderId);
            }

            return Result<int>.Success(newOrder.OrderId);
        }
        public async Task<Result<int>> CancelOrder(int OrderId, CancelOrder cancelOrder, CancellationToken cancellationToken)
        {
            if (cancelOrder == null)
            {
                return Result<int>.Failure(new AppError(ErrorType.Validation, "INVALID_CANCELLATION", "Cancellation request cannot be null."));
            }
            if (cancelOrder.OrderId != OrderId)
            {
                return Result<int>.Failure(new AppError(ErrorType.Validation,"ORDER_ID_MISMATCH","OrderId does not match the cancellation request."));
            }
            await using var transaction =await _context.Database.BeginTransactionAsync(cancellationToken);
            var order = await _context.Orders.Include(order => order.Items).Include(order => order.Customer).FirstOrDefaultAsync(order => order.OrderId == OrderId, cancellationToken);
            if (order == null)
            {
                await transaction.RollbackAsync(cancellationToken);

                return Result<int>.Failure(new AppError(ErrorType.NotFound,"ORDER_NOT_FOUND",$"Order {OrderId} was not found."));
            }
            int affectedRows = await _context.Orders.Where(order => order.OrderId == OrderId && order.status == 0).ExecuteUpdateAsync( setters => setters.SetProperty(order => order.status, 2),cancellationToken);

            if (affectedRows != 1)
            {
                await transaction.RollbackAsync(cancellationToken);

                return Result<int>.Failure(new AppError(ErrorType.Conflict,"ORDER_CANNOT_BE_CANCELLED","Order is already cancelled or cannot be cancelled."));
            }
           
            foreach (var item in order.Items)
            {
                var productAffectedRows = await _context.Products.Where(product => product.ProductId == item.ProductId).ExecuteUpdateAsync(setters => setters.SetProperty(p=>p.AvailableStock, p => p.AvailableStock + item.OrderQuantity), cancellationToken);
               if(productAffectedRows != 1)
    {
                    throw new InvalidOperationException(  $"failed to restore stock for product {item.ProductId}");
                }
            }
            await _context.SaveChangesAsync(cancellationToken); 
            await transaction.CommitAsync(cancellationToken);

            try
            {
                await distributedCache.RemoveAsync("products:v2:all", CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Failed to invalidate products list after order cancellation.");
            }

            foreach (var item in order.Items)
            {
                try
                {
                    await distributedCache.RemoveAsync($"products:v2:id:{item.ProductId}", CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,"Failed to invalidate product {ProductId} after order cancellation.",item.ProductId);
                }
            }
            
            try
            {
                var channel = order.Customer.CustomerPrefferdChannel;
                var recipient = channel switch
                {
                    NotificationChannel.Email => order.Customer.CustomerEmail,
                    NotificationChannel.SMS => order.Customer.CustomerPhone,
                    _ => throw new NotSupportedException("Notification channel is not supported.")
                };
               

                if (!string.IsNullOrWhiteSpace(recipient))
                {
                    var notification = new NotificationMessage(
                        Recipient: recipient,
                        Subject: $"order with id #{order.OrderId} cancelled",
                        Body: $"Your order with id#{order.OrderId} has been cancelled successfully.",
                        Channel: channel
                    );

                    await notificationService.SendAsync(notification,CancellationToken.None);
                }
                else
                {
                    _logger.LogWarning("no notification available for order with order id {OrderId}.", order.OrderId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,"notification failed after cancelling order {OrderId}.", order.OrderId);
            }

            return Result<int>.Success(OrderId);
        }
    }
}
