using Microsoft.AspNetCore.Diagnostics;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Exceptions
{
    public class OrderConflictExceptionHandler: IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync( HttpContext httpContext, Exception exception,CancellationToken cancellationToken)
        {
            if (exception is not OrderStateConflictException)
            {
                return false;
            }

            httpContext.Response.StatusCode =StatusCodes.Status409Conflict;

            await httpContext.Response.WriteAsJsonAsync(
                new
                {
                    Message = exception.Message
                },cancellationToken);

            return true;
        }
    }
}
