namespace High_Traffic_E_Commerce_Inventory_Order_System.models.ProductsModel
{
    public class Product
    {
        public int ProductId { get; init; }
        public string ProductName { get; set; }=string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public bool IsActive { get; set; } = true;
        public string SKU { get; set; } = string.Empty;
        public int AvailableStock { get; set; }
        public int RowVersion { get; set; } = 1;

    }
}
