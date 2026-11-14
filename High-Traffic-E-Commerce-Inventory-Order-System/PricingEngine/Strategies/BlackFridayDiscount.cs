using High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Interfaces;

namespace High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Strategies
{
    public class BlackFridayDiscount : IPricingPlanStrategy
    {
        public decimal PayAmount(decimal amount)
        {
            decimal discount = 0.4M; 
            decimal amountOfDiscount = amount * discount;
            decimal finalAmount = amount - amountOfDiscount;
            return finalAmount;
        }
    }
}
