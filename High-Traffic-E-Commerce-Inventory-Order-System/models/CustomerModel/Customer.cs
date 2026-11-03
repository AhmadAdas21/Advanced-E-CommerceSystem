using High_Traffic_E_Commerce_Inventory_Order_System.models.OrderModel;

namespace High_Traffic_E_Commerce_Inventory_Order_System.models.customer_model
{
    public class Customer
    {
        public int customerId { get; init; }
        public string customerName { get; set; }
        public string customerEmail { get; set; }
        public string customerPhone { get; set; }
        List<Order> orders { get; set; }

    }
}
