using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;
using System.Text;
using System.Text.Json;

namespace WebService
{
    public class Startup
    {

        IWebHostEnvironment _env;
        public Startup(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();

            var routeBuilder = new RouteBuilder(app);
            routeBuilder.MapMiddlewarePost("makeTrack", app =>
            {
                app.Run(async context =>
                {
                    await makeTrack(context);
                });
            });
            app.UseRouter(routeBuilder.Build());

            routeBuilder = new RouteBuilder(app);
            routeBuilder.MapMiddlewarePost("addMessage", app =>
            {
                app.Run(async context =>
                {
                    await addMessage(context);
                });
            });
            app.UseRouter(routeBuilder.Build());


            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("<h1>YOU CONNECTED<h1/>");
            });
        }

        private async Task makeTrack(HttpContext context)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.WriteAsync(dbContainer.GetObjects());
        }

        private async Task addMessage(HttpContext context)
        {
            byte[] buf = new byte[(int)context.Request.ContentLength]; 

            await context.Request.Body.ReadAsync(buf,0, (int)context.Request.ContentLength);

            var res = dbContainer.SetObject(Encoding.ASCII.GetString(buf));
            if (res != null)
            {
                await context.Response.Body.WriteAsync(Encoding.Default.GetBytes(res));
                return;
            }

            await context.Response.Body.WriteAsync(buf);
        }

    }
}

