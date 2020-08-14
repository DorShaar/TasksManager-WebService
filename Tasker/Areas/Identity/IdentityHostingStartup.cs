using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tasker.Areas.Identity.Data;
using Tasker.Data;

[assembly: HostingStartup(typeof(Tasker.Areas.Identity.IdentityHostingStartup))]
namespace Tasker.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        const string SQLConnectionString = "TaskerContextConnection";

        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<TaskerContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString(SQLConnectionString)));

                services.AddDefaultIdentity<TaskerUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<TaskerContext>();
            });
        }
    }
}