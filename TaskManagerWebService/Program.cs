using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TaskManagerWebService.Persistence.Context;

namespace TaskManagerWebService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHost webHost = CreateWebHost(args);

            using (IServiceScope scope = webHost.Services.CreateScope())
            using (AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                context.Database.EnsureCreated();
            }

            webHost.Run();
        }

        public static IWebHost CreateWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}