using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Sharpcms.Base.Core
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSharpcms(this IApplicationBuilder app)
        {
            app.UseMiddleware<SharpcmsMiddleware>();
            return app;
        }
    }

    public class SharpcmsMiddleware
    {
        private readonly RequestDelegate _next;

        public SharpcmsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await Sharpcms.Send(context);
            await _next(context);
        }
    }
}
