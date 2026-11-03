using System.ComponentModel.DataAnnotations;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.CustomerDto_s
{
    public record GetCustomers
    {
        [Required]
        public int CustomerId { get; init; }
        [Required]
        public string CustomerName { get; init; }
        [Required]
        public string CustomerPhone { get; init; }
        [Required]
        public string CustomerEmail { get; init; }

    }
}
