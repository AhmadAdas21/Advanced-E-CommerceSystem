using System.ComponentModel.DataAnnotations;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s
{
    public class PatchProduct
    {
        [MaxLength(40)]
        public string? ProductName { get; set; }
        [MaxLength(200)]
        public string? ProductDescription { get; set; }
        [Range(0.5, 1000000)]
        public decimal? ProductPrice { get; set; }
        [Range(0,1000000)]
        public int? AvailableStock { get; set; }
        [MaxLength(20)]
        public string? SKU { get; set; }
        public bool? IsActive { get; set; }


    }
}
