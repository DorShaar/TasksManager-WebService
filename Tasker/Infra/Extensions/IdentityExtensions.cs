using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tasker.Data;
using Tasker.Domain.Models;

namespace Tasker.Infra.Extensions
{
    public static class IdentityExtensions
    {
        public static void UseIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TaskerDbContext>(options =>
                options.UseSqlite(
                    configuration.GetConnectionString("TaskerContextConnection")));

            services.AddDefaultIdentity<TaskerUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<TaskerDbContext>();

            services.AddIdentityServer().AddApiAuthorization<TaskerUser, TaskerDbContext>();

            services.AddAuthentication().AddIdentityServerJwt();
        }
    }
}