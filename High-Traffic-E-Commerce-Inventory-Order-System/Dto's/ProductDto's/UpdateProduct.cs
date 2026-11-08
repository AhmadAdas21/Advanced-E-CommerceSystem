using System.ComponentModel.DataAnnotations;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s
{
    public record UpdateProduct
    {
        [Required]
        [MaxLength(40)]
        public string? ProductName { get; set; }
        [Required]
        [MaxLength(200)]
        public string? ProductDescription { get; set; }
        [Required]
        [Range(0.5,1000000)]
        public decimal ProductPrice { get; set; }
        [Required]
        [Range(0,1000000)]
        public int ProductQuantity { get; set; }
    }
}
