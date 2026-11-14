using High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Interfaces;

namespace High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Stratigies
{
    public class VipDiscount:IPricingPlanStrategy
    {
        public decimal PayAmount(decimal amount)
        {
            decimal discount = 0.15M;
            decimal amountOfDiscount = discount * amount;
            decimal finalAmount = amount - amountOfDiscount;
            return finalAmount;
        }
    }
}
