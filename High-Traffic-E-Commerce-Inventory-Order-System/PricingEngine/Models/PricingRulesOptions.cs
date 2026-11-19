namespace High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Models
{
    public sealed class PromotionWindow
    {
        public bool Enabled { get; set; }
        public DateTimeOffset StartUtc { get; set; }
        public DateTimeOffset EndUtc { get; set; }

        public bool IsActive(DateTimeOffset now)
        {
            return Enabled&& StartUtc < EndUtc&& now >= StartUtc&& now < EndUtc;
        }
    }

    public sealed class PricingRulesOptions
    {
        public PromotionWindow FlashSale { get; set; } = new();
        public PromotionWindow Seasonal { get; set; } = new();
    }
}
