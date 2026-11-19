namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s
{
    public record CachedProduct
    {
        public int ProductId { get; init; }
        public string? SKU { get; init; }
        public string? ProductName { get; init; }
        public string? ProductDescription { get; init; }
        public decimal? ProductPrice { get; init; }
        public bool IsActive { get; init; }
    }
}
