using System.ComponentModel.DataAnnotations;
using High_Traffic_E_Commerce_Inventory_Order_System.models.OrderModel;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.CustomerDto_s
{
    public record CreateCustomer
    {
        [Required]
        public string CustomerName { get; init; } = string.Empty;
        [Required]
        [EmailAddress]
        public string CustomerEmail { get; init; } = string.Empty;
        [Required]
        [MaxLength(10)]
        [MinLength(10)]
        public string CustomerPhone { get; init; } = string.Empty;

    }
}
