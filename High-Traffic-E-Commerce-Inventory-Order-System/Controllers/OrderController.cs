using High_Traffic_E_Commerce_Inventory_Order_System.Auditing;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.ResultPattern;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController:ControllerBase
    {
        private readonly IOrderServices orderServices;
        public OrderController(IOrderServices orderServices)
        {
            this.orderServices = orderServices;
        }
        [HttpGet]
        public async Task<IActionResult>GetOrders(CancellationToken cancellationToken)
        {
            var orders= await orderServices.GetOrders(cancellationToken);
            return Ok(orders.Value);
        }
        [HttpGet("{orderId:int}")]
        public async Task<IActionResult>SearchOrder([FromRoute(Name = "orderId")] int OrderId,CancellationToken cancellationToken)
        {
            var order =await orderServices.SearchOrder(OrderId,cancellationToken);
            if (order.IsSuccess)
            {
                return Ok(order.Value);
            }

            var error = order.Error;

            return error.Type switch
            {
                ErrorType.NotFound => NotFound(new
                {
                    error.Code,
                    error.Message
                }),

                ErrorType.Validation => BadRequest(new
                {
                    error.Code,
                    error.Message
                }),

                ErrorType.Conflict => Conflict(new
                {
                    error.Code,
                    error.Message
                }),

                _ => StatusCode(500)
            };
        }
        
        [Auditable("CreateOrder")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrder order, CancellationToken cancellationToken)
        {
            var createdOrder = await orderServices.CreateOrder(order, cancellationToken);
            if (createdOrder.IsSuccess)
            {
                return StatusCode(StatusCodes.Status201Created, new
                {
                    Message = "Order created successfully",
                    OrderId = createdOrder.Value
                });
            }

            var error = createdOrder.Error;

            return error.Type switch
            {
                ErrorType.Validation => BadRequest(new { error.Code, error.Message }),
                ErrorType.NotFound => NotFound(new { error.Code, error.Message }),
                ErrorType.Conflict => Conflict(new { error.Code, error.Message }),
                _ => StatusCode(500)
            };
            }
        [Auditable("CancelOrder")]
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CancelOrder(int orderId, [FromBody] CancelOrder order, CancellationToken cancellationToken)
        {
            var cancelledOrder = await orderServices.CancelOrder(orderId, order, cancellationToken);
            if (cancelledOrder.IsSuccess)
            {
                return Ok(new
                {
                    Message = "Order cancelled successfully",
                    OrderId = cancelledOrder.Value
                });
            }

            var error = cancelledOrder.Error;

            return error.Type switch
            {
                ErrorType.Validation => BadRequest(new
                {
                    error.Code,
                    error.Message
                }),

                ErrorType.NotFound => NotFound(new
                {
                    error.Code,
                    error.Message
                }),

                ErrorType.Conflict => Conflict(new
                {
                    error.Code,
                    error.Message
                }),

                _ => StatusCode(500)
            };
        }
        
        
    }
}
