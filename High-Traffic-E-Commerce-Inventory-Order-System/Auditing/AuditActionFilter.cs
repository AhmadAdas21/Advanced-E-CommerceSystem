using Microsoft.AspNetCore.Mvc.Filters;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Auditing
{
    public class AuditActionFilter : IAsyncActionFilter
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuditActionFilter> _logger;
        public AuditActionFilter(IServiceScopeFactory scopeFactory,ILogger<AuditActionFilter> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
    }
}
