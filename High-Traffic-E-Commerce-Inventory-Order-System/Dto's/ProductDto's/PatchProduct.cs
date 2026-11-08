namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s
{
    public class PatchProduct
    {
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public decimal? ProductPrice { get; set; }
        public int? ProductStock { get; set; }
        public string? SKU { get; set; }
        public bool? IsActive { get; set; }


    }
}
