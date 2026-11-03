using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.CustomerDto_s;
namespace High_Traffic_E_Commerce_Inventory_Order_System.Services
{
    public class CustomerServices : ICustomerServices
    {
        private readonly DataBaseContext _dataBase; 
        public CustomerServices(DataBaseContext dataBase) 
        {
            _dataBase = dataBase;
        }
        public async Task GetAllCustomers()
        {
            var customers=await _dataBase.Customers.ToListAsync();
            if (customers == null || !customers.Any())
            {
                throw new Exception("no customers found.");
            }
            return customers;

        }
        public async Task CreateProduct(CreateCustomer customer,CancellationToken cancellationToken)
        {

        }
    }
}
