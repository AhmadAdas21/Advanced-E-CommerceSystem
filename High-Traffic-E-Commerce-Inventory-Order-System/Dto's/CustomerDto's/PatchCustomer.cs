namespace High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.CustomerDto_s
{
    public record PatchCustomer
    {
        
        public string? CustomerName { get; init; }
        public string? CustomerPhone { get; init; }
        public string? CustomerEmail { get; init; }
    }
}
