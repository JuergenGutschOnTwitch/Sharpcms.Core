using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Sharpcms.Core
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });

            app.UseSharpcms();
        }
    }

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
