using High_Traffic_E_Commerce_Inventory_Order_System.models.OrderModel;

namespace High_Traffic_E_Commerce_Inventory_Order_System.models.customer_model
{
    public class Customer
    {
        public int CustomerId { get; init; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public List<Order> Orders { get; set; } = new List<Order>();

    }
}
