using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.CustomerDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Controllers
{
    [ApiController]
    [Route("api/customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerServices customerServices;

        public CustomerController(ICustomerServices customerServices)
        {
            this.customerServices = customerServices;

        }
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {

            var customers = await customerServices.GetAllCustomers();
            if (customers == null)
            {
                return NotFound("no customers found");
            }
            return Ok(customers);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomer customer, CancellationToken cancellationToken)
        {
            await customerServices.CreateCustomer(customer, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new { Message = "Customer created successfully" });

        }
        [HttpPut("{customerId}")]
        public async Task<IActionResult>UpdateCustomer(int customerId, [FromBody]UpdateCustomer customer, CancellationToken cancellationToken)
        {
            await customerServices.UpadateCustomer(customerId, customer, cancellationToken);
            return Ok("the customer updated successfully");

        }
        [HttpDelete("{customerId}")]
        public async Task<IActionResult>DeleteCustomer(int customerId , CancellationToken canellationToken)
        {
            await customerServices.DeleteCustomer(customerId, canellationToken);
            return Ok("the customer deleted successfully");
        }
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerById(int customerId,CancellationToken cancellationToken)
        {
            await customerServices.GetCustomerById(customerId, cancellationToken);
            return Ok("the customer found successfully");
        }   

    }
}
