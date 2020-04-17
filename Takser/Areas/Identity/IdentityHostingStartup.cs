using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tasker.Data;
using Tasker.Domain.Models;

[assembly: HostingStartup(typeof(Takser.Areas.Identity.IdentityHostingStartup))]
namespace Takser.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<TaskerContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("TaskerContextConnection")));

                services.AddDefaultIdentity<ApplicationUser>()
                    .AddEntityFrameworkStores<TaskerContext>();
            });
        }
    }
}