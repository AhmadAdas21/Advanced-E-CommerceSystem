using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.CustomerDto_s;
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
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
           var customers=await customerServices.GetAllCustomers();
            return Ok(customers);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody]CreateCustomer customer,CancellationToken cancellationToken)
        {
            var createdCustomer = await customerServices.CreateCustomer(customer);
            return Ok(createdCustomer);

        }
}
