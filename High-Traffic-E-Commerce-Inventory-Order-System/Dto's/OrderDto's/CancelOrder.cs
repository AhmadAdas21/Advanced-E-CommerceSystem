using System.ComponentModel.DataAnnotations;
using High_Traffic_E_Commerce_Inventory_Order_System.models.OrderModel;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s
{
    public record CancelOrder
    {
        [Required]
        public int OrderId { get; init; }
        [Required]
        public int Status { get; set; }


    }
}
