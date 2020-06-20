using FakeItEasy;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO;
using Takser.Infra.Options;
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

        public static TestServer CreateTestServerWithFakeDatabaseConfig()
        {
            IOptions<DatabaseConfigurtaion> defaultDatabaseConfig = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = Path.Combine("TestFiles", "GeneralNotes"),
                NotesTasksDirectoryPath = Path.Combine("TestFiles", "TaskNotes"),
            });

            var testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .ConfigureTestServices(sc =>
                {
                    sc.AddSingleton(defaultDatabaseConfig);
                })
                .UseStartup<Startup>()
                .UseEnvironment("Development"));

            return testServer;
        }

        public static TestServer BuildTestServerWithFakes(ITasksGroupService tasksGroupService = null, IWorkTaskService workTaskService = null,
            INoteService noteService = null)
        {
            if (tasksGroupService == null)
                tasksGroupService = A.Fake<ITasksGroupService>();

            if (workTaskService == null)
                workTaskService = A.Fake<IWorkTaskService>();

            if (noteService == null)
                noteService = A.Fake<INoteService>();

            TestServer testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .ConfigureTestServices(sc =>
                {
                    sc.AddSingleton(tasksGroupService);
                    sc.AddSingleton(workTaskService);
                    sc.AddSingleton(noteService);
                })
                .UseStartup<Startup>()
                .UseEnvironment("Development"));

            return testServer;
        }
    }
}