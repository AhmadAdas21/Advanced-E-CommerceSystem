using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.CustomerDto_s;

namespace High_Traffic_E_Commerce_Inventory_Order_System.interfaces
{
    public interface ICustomerServices
    {
        public Task GetAllCustomers();
        public Task CreateCustomer(CreateCustomer customer,CancellationToken cancellationToken);
        public Task UpadateCustomer(int customerId,UpdateCustomer customer ,CancellationToken cancellationToken);
        public Task DeleteCustomer(int customerId, CancellationToken cancellationToken);
    }
}
