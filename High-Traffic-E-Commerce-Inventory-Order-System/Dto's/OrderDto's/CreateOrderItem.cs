using System.ComponentModel.DataAnnotations;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.OrderDto_s
{
    public record CreateOrderItem
    {
        [Required]
        [MaxLength(20)]
        public string SKU { get; init; } = string.Empty;

        [Range(1, 100000)]
        public int Quantity { get; init; }
    }
}
