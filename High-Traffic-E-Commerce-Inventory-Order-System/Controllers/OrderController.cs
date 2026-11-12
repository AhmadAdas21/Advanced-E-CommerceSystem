using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
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
            return Ok(orders);
        }
        [HttpGet("{orderId:int}")]
        public async Task<IActionResult>SearchOrder([FromRoute(Name = "orderId")] int OrderId,CancellationToken cancellationToken)
        {
            var order =await orderServices.SearchOrder(OrderId,cancellationToken);
            return Ok(order);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody]CreateOrder order, CancellationToken cancellationToken)
        {
            await orderServices.CreateOrder(order,cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new { Message = "order created successfully" });
        }
        [HttpPatch]
        public async Task<IActionResult>CancelOrder(int orderId,[FromBody] CancelOrder order,CancellationToken cancellationToken )
        {
            await orderServices.CancelOrder(orderId, order, cancellationToken);
            return Ok();
        }
        
        
    }
}
