using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Tasker.App.Services;

namespace Tasker.Tests.Api.Controllers
{
    internal static class ApiTestHelper
    {
        public static TestServer CreateTestServer()
        {
            var testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseEnvironment("Development"));

            return testServer;
        }

        public static TestServer CreateTestServerWithFakes(ITasksGroupService tasksGroupService, IWorkTaskService workTaskService)
        {
            var testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .ConfigureTestServices(sc =>
                {
                    sc.AddSingleton(tasksGroupService);
                    sc.AddSingleton(workTaskService);
                })
                .UseStartup<Startup>()
                .UseEnvironment("Development"));

            return testServer;
        }
    }
}