using High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Interfaces;

namespace High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine
{
    public class PricingEngine : IPricingEngine
    {
        private readonly IPricingPlanStrategy planStrategy;
        private readonly IPricingEngine pricingEngine;

       
        public PricingEngine(IPricingPlanStrategy planStrategy, IPricingEngine pricingEngine)
        {
            this.pricingEngine = pricingEngine;
            this.planStrategy = planStrategy;
        }

        
        public decimal CalculatePrice(IPricingPlanStrategy plan, decimal originalPrice)
        {
          
            return plan.CalculatePrice(originalPrice);
        }
    }
}
