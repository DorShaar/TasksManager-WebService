using FakeItEasy;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Takser;
using TaskData.Contracts;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Xunit;

namespace Tasker.Tests.Api.Controllers
{
    public class TasksGroupsControllerTests
    {
        private const string MainRoute = "api/TasksGroups";

        [Fact]
        public async Task Groups_SuccessStatusCode()
        {
            using TestServer testServer = CreateTestServerWithFakes(A.Fake<ITasksGroupService>(), A.Fake<IWorkTaskService>());
            using HttpClient httpClient = testServer.CreateClient();
            HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}/Groups");

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("1000")]
        public async Task RemoveGroupAsync_GroupExistAndHasSizeGreaterThanOne_GroupNotRemoved(string id)
        {
            using TestServer testServer = CreateTestServer();
            using HttpClient httpClient = testServer.CreateClient();
            HttpResponseMessage response = await httpClient.DeleteAsync($"{MainRoute}/{id}");

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task RemoveGroupAsync_RemoveSuccess_SuccessStatusCode()
        {
            ITasksGroupService fakeTasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => fakeTasksGroupService.RemoveAsync(A<string>.Ignored))
                .Returns(new Response<ITasksGroup>(isSuccess: true, ""));

            using TestServer testServer = CreateTestServerWithFakes(fakeTasksGroupService, A.Fake<IWorkTaskService>());
            using HttpClient httpClient = testServer.CreateClient();
            HttpResponseMessage response = await httpClient.DeleteAsync($"{MainRoute}/some-id");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RemoveGroupAsync_ThrowsException_InternalServerErrorStatusCode()
        {
            ITasksGroupService fakeTasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => fakeTasksGroupService.RemoveAsync(A<string>.Ignored))
                .Throws<Exception>();

            using TestServer testServer = CreateTestServerWithFakes(fakeTasksGroupService, A.Fake<IWorkTaskService>());
            using HttpClient httpClient = testServer.CreateClient();
            HttpResponseMessage response = await httpClient.DeleteAsync($"{MainRoute}/some-id");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        private TestServer CreateTestServer()
        {
            var testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseEnvironment("Development"));

            return testServer;
        }

        private TestServer CreateTestServerWithFakes(ITasksGroupService tasksGroupService, IWorkTaskService workTaskService)
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