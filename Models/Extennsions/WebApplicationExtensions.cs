using Microsoft.AspNetCore.Builder;
using Models.CustomMiddleWares;

namespace Models.Extennsions
{
    public static class WebApplicationExtensions
    {
        public static IApplicationBuilder UserAdminRedirection(this IApplicationBuilder app)
        {
            app.UseMiddleware<AdminRedirectionMiddleware>();
            return app;
        }
    }
}
