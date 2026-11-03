using High_Traffic_E_Commerce_Inventory_Order_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Controllers
{
    [ApiController]
    [Route("api/customer]")]
    public class CustomerController:ControllerBase
    {
        private readonly CustomerServices customerServices;
        public CustomerController(CustomerServices customerServices)
        {
            this.customerServices = customerServices;
        }
    }
}
