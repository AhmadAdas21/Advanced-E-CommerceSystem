using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
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
        public async Task OnActionExecutionAsync(ActionExecutingContext context,ActionExecutionDelegate next)
        {
            var descriptor =context.ActionDescriptor as ControllerActionDescriptor;

            var attribute = descriptor?.MethodInfo.GetCustomAttribute<AuditableAttribute>();

            if (attribute == null)
            {
                await next();
                return;
            }

            ActionExecutedContext executedContext;

            try
            {
                executedContext = await next();
            }
            catch (Exception)
            {
                await SaveAuditAsync(attribute.Operation, "Exception", 500);
                throw;
            }

            bool hasException = executedContext.Exception != null &&!executedContext.ExceptionHandled;

            int statusCode = hasException ? 500: executedContext.Result switch
                {
                    ObjectResult result => result.StatusCode ?? 200, StatusCodeResult result => result.StatusCode,
                    _ => executedContext.HttpContext.Response.StatusCode
                };

            string outcome = hasException ? "Exception": statusCode >= 400 ? "Rejected" : "Success";

            await SaveAuditAsync(attribute.Operation, outcome, statusCode);
        }

        private async Task SaveAuditAsync(string operation,string outcome,int statusCode)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();

                var db = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

                db.AuditLogs.Add(new AuditLog
                {
                    Operation = operation,
                    TimestampUtc = DateTimeOffset.UtcNow,
                    Outcome = outcome,
                    HttpStatusCode = statusCode
                });

                await db.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Failed to save audit log for {Operation}",operation);
            }
        }

    }
}
