using High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Interfaces;

namespace High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Strategies
{
    public class NormalDiscount:IPricingPlanStrategy
    {
        public decimal PayAmount(decimal amount)
        {
            decimal discount = 0;
            decimal amountOfDiscount = discount * amount;
            decimal finalPrice = amount - amountOfDiscount;
            return finalPrice;
        }
    }
}
