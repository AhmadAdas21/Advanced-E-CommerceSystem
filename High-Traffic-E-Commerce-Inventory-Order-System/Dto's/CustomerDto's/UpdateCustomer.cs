using System.Reflection.Metadata;
using High_Traffic_E_Commerce_Inventory_Order_System.models.OrderModel;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.CustomerDto_s
{
    public record UpdateCustomer
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public List<Order> Orders { get; set; }

    }
}
