namespace High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Stratigies
{
    public class SeasonalDiscount
    {
        public decimal PayAmount(decimal amount)
        {
            decimal discount = 0.2M;
            decimal amountOfDiscount = amount * discount;
            decimal finalAmount = amount - amountOfDiscount;
            return finalAmount;
        }
    }
}
