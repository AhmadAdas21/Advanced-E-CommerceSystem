using System.ComponentModel.DataAnnotations;
using High_Traffic_E_Commerce_Inventory_Order_System.models.customer_model;
using High_Traffic_E_Commerce_Inventory_Order_System.models.OrderModel;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s
{
    public record CreateOrder
    {
        [Range(1, 1000000)]
        public int CustomerId { get; init; }

        [Required]
        [MinLength(1)]
        public List<CreateOrderItem> Items { get; init; } = new();




    }
}
