using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Tasker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHost webHost = CreateWebHost(args);
            webHost.Run();
        }

        public static IWebHost CreateWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
