using High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Strategies;

namespace High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine
{
    public class PricingEngine : IPricingEngine
    {
     

        
        public decimal CalculatePrice(IPricingPlanStrategy plan, decimal originalPrice)
        {
          
            return plan.PayAmount(originalPrice);
        }
    }
}
