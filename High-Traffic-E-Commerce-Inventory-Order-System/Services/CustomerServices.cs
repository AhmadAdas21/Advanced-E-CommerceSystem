using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.CustomerDto_s;
using Microsoft.EntityFrameworkCore;
using High_Traffic_E_Commerce_Inventory_Order_System.models.customer_model;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Threading;
namespace High_Traffic_E_Commerce_Inventory_Order_System.Services
{
    public class CustomerServices : ICustomerServices
    {
        private readonly DataBaseContext _dataBase; 
        public CustomerServices(DataBaseContext dataBase) 
        {
            _dataBase = dataBase;
        }
        public async Task<List<Customer>> GetAllCustomers()
        {
            var customers=await _dataBase.Customers.ToListAsync();
            if (customers == null || !customers.Any())
            {
                throw new Exception("no customers found.");
            }
            return customers;

        }
        public async Task CreateCustomer(CreateCustomer customer,CancellationToken cancellationToken)
        {
            if(customer==null)
            {
                throw new ArgumentNullException(nameof(customer), "customer cannot be null.");
            }
            var newCustomer = new Customer
            {
                CustomerName = customer.CustomerName,
                CustomerEmail = customer.CustomerEmail,
                CustomerPhone = customer.CustomerPhone

            };
            await _dataBase.Customers.AddAsync(newCustomer);
            await _dataBase.SaveChangesAsync(cancellationToken);    


        }
        public async Task UpadateCustomer(int customerId,UpdateCustomer updateCusotmer,CancellationToken cancellationToken)
        {
            var customer=await _dataBase.Customers.FirstOrDefaultAsync(customer=>customer.CustomerId == customerId);
            if (customer == null)
            {
                throw new Exception("customer not found");
            }
            customer.CustomerName = updateCusotmer.CustomerName;
            customer.CustomerEmail=updateCusotmer.CustomerEmail;
            customer.CustomerPhone = updateCusotmer.CustomerPhone;
            await _dataBase.SaveChangesAsync(cancellationToken);
           

        }
        public async Task DeleteCustomer(int id, CancellationToken cancellationToken)
        {
            var customer= await _dataBase.Customers.FirstOrDefaultAsync(customer => customer.CustomerId == id);
            if (customer == null)
            {
                throw new Exception("customer not found");
            }
            _dataBase.Remove(customer);
            await _dataBase.SaveChangesAsync(cancellationToken);
        }
        public async Task <GetCustomers>GetCustomerById(int customerId, CancellationToken cancellationToken)
        {
            var customer=await _dataBase.Customers.FirstOrDefaultAsync(customer => customer.CustomerId == customerId);
            if (customer == null)
            {
                throw new Exception("customer not found");
            }
            return customer;
        }
    }
}
