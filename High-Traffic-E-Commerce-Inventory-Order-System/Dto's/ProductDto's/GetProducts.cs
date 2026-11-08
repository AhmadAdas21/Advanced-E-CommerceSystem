using System.ComponentModel.DataAnnotations;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s
{
    public record GetProducts
    {
        [Required]
        public string? SKU { get; init; }
        [Required]
        public string ?ProductName { get; init; }
        [Required]
        public string? ProductDescription { get; init; }
        [Required]
        public decimal? ProductPrice { get; init; }
        

    }
}
