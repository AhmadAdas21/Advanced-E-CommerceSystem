using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
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
            await orderServices.GetOrders(cancellationToken);
            return Ok();
        }
        [HttpGet()]

    }
}
