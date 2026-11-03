using System.ComponentModel.DataAnnotations;
using High_Traffic_E_Commerce_Inventory_Order_System.models.OrderModel;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s
{
    public record CreateOrder
    {
        [Required]
        public int CustomerId { get; init; }
        [Required]
        public List<OrderItem> Items { get; init; } = new List<OrderItem>();



    }
}
