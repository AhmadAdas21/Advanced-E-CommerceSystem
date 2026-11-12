using System.ComponentModel.DataAnnotations;
using High_Traffic_E_Commerce_Inventory_Order_System.models.OrderModel;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s
{
    public record UpdateOrder
    {
        [Required]
        public List<OrderItem> OrderItem { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int Status { get; set; }

    }
}
