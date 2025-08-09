using Microsoft.AspNetCore.Http;

namespace Models.CustomMiddleWares
{
    public class AdminRedirectionMiddleware
    {
        private readonly RequestDelegate _next;
        public AdminRedirectionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true && context.Request.Path == "/")
            {
                if(context.User.IsInRole("Admin"))
                {
                    context.Response.Redirect("/Admin/Home/Index");
                    return;
                }
            }
            await _next(context);
        }
    }
}
