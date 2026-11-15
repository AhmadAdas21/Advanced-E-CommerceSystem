using High_Traffic_E_Commerce_Inventory_Order_System.models.customer_model;
using High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Models;
using High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine.Strategies;
using Microsoft.Extensions.Options;

namespace High_Traffic_E_Commerce_Inventory_Order_System.PricingEngine
{
    public class PricingStrategySelector:IPricingStrategySelector
    {
        private readonly PricingRulesOptions _rules;

        public PricingStrategySelector(IOptions<PricingRulesOptions> options)
        {
            _rules = options.Value;
        }

        public IPricingPlanStrategy Select(Customer customer,DateTimeOffset now)
        {
            return (
                _rules.FlashSale.IsActive(now),
                _rules.Seasonal.IsActive(now),
                customer.IsVip
            ) switch
            {
                (true, _, _) => new BlackFridayDiscount(),
                (_, true, _) => new SeasonalDiscount(),
                (_, _, true) => new VipDiscount(),
                _ => new NormalDiscount()
            };
        }
    }
}
