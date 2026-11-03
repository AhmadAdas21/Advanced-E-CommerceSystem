using High_Traffic_E_Commerce_Inventory_Order_System.models.customer_model;

namespace High_Traffic_E_Commerce_Inventory_Order_System.models.OrderModel
{
    public class Order
    {
        public int OrderId { get; init; }
        public int CustomerId { get; set; }
        public List<OrderItem> Items { get; set; }
        public decimal TotalAmount { get; set; } = 0;
        public Customer Customer { get; set; }


    }
}
