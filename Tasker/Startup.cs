using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tasker.Api.Middlewares;
using Tasker.Infra.Extensions;

namespace Tasker
{
    public class Startup
    {
        private const string TaskerCorsPolicy = "TaskerCorsPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            // Adds "Access-Control-Allow-(Origin/Methods/Headers)" header to response.
            services.AddCors(options =>
            {
                options.AddPolicy(TaskerCorsPolicy, builder =>
                {
                    builder.WithOrigins("*")
                         .AllowAnyMethod()
                         .AllowAnyHeader();
                });
            });

            services.UseDI();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMiddleware<RequestLoggingMiddleware>()
               .UseMiddleware<ExceptionHandlingMiddleware>()
               .UseHttpsRedirection()
               .UseStaticFiles()
               .UseRouting()
               .UseCors(TaskerCorsPolicy)
               .UseAuthentication()
               .UseAuthorization()
               .UseEndpoints(endpoints =>
               {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
               });
        }
    }
}