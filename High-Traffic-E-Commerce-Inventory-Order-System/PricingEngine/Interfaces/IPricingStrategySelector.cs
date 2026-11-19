using High_Traffic_E_Commerce_Inventory_Order_System.models.customer_model;

namespace High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Interfaces
{
    public interface IPricingStrategySelector
    {
        IPricingPlanStrategy SelectPricingStrategy(Customer customer,DateTimeOffset timeNow);
    }
}
