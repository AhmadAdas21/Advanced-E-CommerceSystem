using System.Threading;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.CustomerDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.models.customer_model;

namespace High_Traffic_E_Commerce_Inventory_Order_System.interfaces
{
    public interface ICustomerServices
    {
        public Task<List<Customer>> GetAllCustomers();
        public Task CreateCustomer(CreateCustomer customer,CancellationToken cancellationToken);
        public Task UpadateCustomer(int customerId,UpdateCustomer customer ,CancellationToken cancellationToken);
        public Task DeleteCustomer(int customerId, CancellationToken cancellationToken);
        public Task<GetCustomers> GetCustomerById(int customerId, CancellationToken cancellationToken);
    }
}
