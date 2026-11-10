using System.ComponentModel.DataAnnotations;
using High_Traffic_E_Commerce_Inventory_Order_System.models.customer_model;
using High_Traffic_E_Commerce_Inventory_Order_System.models.OrderModel;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s
{
    public record GetOrders
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        [Required]
        public string CustomerName { get; set; }



    }
}
