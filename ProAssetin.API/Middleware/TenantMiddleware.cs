using System.Security.Claims;
using ProAssetin.API.Services;

namespace ProAssetin.API.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantResolver tenantResolver)
        {
            // Extract tenant ID from user claims (set during login)
            var tenantIdClaim = context.User?.FindFirst("TenantId");
            
            if (tenantIdClaim != null)
            {
                var tenantId = tenantIdClaim.Value;
                
                // Ensure tenant database exists
                await tenantResolver.EnsureTenantDatabaseExistsAsync(tenantId);
                
                // Store tenant ID in HttpContext.Items for use in controllers
                context.Items["TenantId"] = tenantId;
            }

            await _next(context);
        }
    }
}

