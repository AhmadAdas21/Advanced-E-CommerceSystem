using System.ComponentModel.DataAnnotations;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s
{
    public record CreateProduct
    {
        [Required]
        [MaxLength(40)]
        public string ProductName { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string ProductDescription { get; set; } = string.Empty;
        [Required]
        [Range(0.5, 100000)]
        public decimal ProductPrice { get; set; } = 0;
        [Required]
        [MaxLength(20)]
        public string SKU { get; set; } = string.Empty;
        [Required]
        public bool? IsActive { get; set; }
        [Required]
        [Range(0,1000000)]
        public int? AvailableStock { get; set; }


    }
}
