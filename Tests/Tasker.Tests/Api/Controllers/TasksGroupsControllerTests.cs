using FakeItEasy;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TaskData.Contracts;
using Tasker.App.Resources;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Xunit;

namespace Tasker.Tests.Api.Controllers
{
    public class TasksGroupsControllerTests
    {
        private const string MainRoute = "api/TasksGroups";
        private const string TasksRoute = "api/TasksGroups/Tasks";
        private const string PostMediaType = "application/json";

        [Fact]
        public async Task ListGroupsAsync_SuccessStatusCode()
        {
            using TestServer testServer = CreateTestServerWithFakes(A.Fake<ITasksGroupService>(), A.Fake<IWorkTaskService>());
            using HttpClient httpClient = testServer.CreateClient();
            HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}");

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ListTasksOfSpecificGroupAsync_groupIdNullOrEmpty_EmptyListReturned(string groupId)
        {
            using TestServer testServer = CreateTestServerWithFakes(A.Fake<ITasksGroupService>(), A.Fake<IWorkTaskService>());
            using HttpClient httpClient = testServer.CreateClient();
            HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}/{groupId}");

            string stringResponse = await response.Content.ReadAsStringAsync();
            IEnumerable<WorkTaskResource> workTaskResources =
                JsonConvert.DeserializeObject<IEnumerable<WorkTaskResource>>(stringResponse);

            Assert.Empty(workTaskResources);
        }

        [Fact]
        public async Task ListTasksOfSpecificGroupAsync_groupNotFound_EmptyListReturned()
        {
            ITasksGroupService tasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => tasksGroupService.ListAsync()).Returns(new List<ITasksGroup>());

            using TestServer testServer = CreateTestServerWithFakes(tasksGroupService, A.Fake<IWorkTaskService>());
            using HttpClient httpClient = testServer.CreateClient();
            HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}/some-id");

            string stringResponse = await response.Content.ReadAsStringAsync();
            IEnumerable<WorkTaskResource> workTaskResources =
                JsonConvert.DeserializeObject<IEnumerable<WorkTaskResource>>(stringResponse);

            Assert.Empty(workTaskResources);
        }

        [Fact]
        public async Task ListTasksOfSpecificGroupAsync_SuccessStatusCode()
        {
            string groupId = "some-id";
            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            A.CallTo(() => tasksGroup.ID).Returns(groupId);

            ITasksGroupService tasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => tasksGroupService.ListAsync()).Returns(new List<ITasksGroup>());

            using TestServer testServer = CreateTestServerWithFakes(tasksGroupService, A.Fake<IWorkTaskService>());
            using HttpClient httpClient = testServer.CreateClient();
            HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}/{groupId}");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ListTasksAsync_NullListReturned_EmptyListReturned()
        {
            List<IWorkTask> tasksList = null;

            IWorkTaskService tasksGroupService = A.Fake<IWorkTaskService>();
            A.CallTo(() => tasksGroupService.ListAsync()).Returns(tasksList);

            using TestServer testServer = CreateTestServerWithFakes(A.Fake<ITasksGroupService>(), tasksGroupService);
            using HttpClient httpClient = testServer.CreateClient();
            HttpResponseMessage response = await httpClient.GetAsync(TasksRoute);

            string stringResponse = await response.Content.ReadAsStringAsync();
            IEnumerable<WorkTaskResource> workTaskResources =
                JsonConvert.DeserializeObject<IEnumerable<WorkTaskResource>>(stringResponse);

            Assert.Empty(workTaskResources);
        }

        [Fact]
        public async Task ListTasksAsync_SuccessStatusCode()
        {
            List<IWorkTask> tasksList = new List<IWorkTask>();

            IWorkTaskService tasksGroupService = A.Fake<IWorkTaskService>();
            A.CallTo(() => tasksGroupService.ListAsync()).Returns(tasksList);

            using TestServer testServer = CreateTestServerWithFakes(A.Fake<ITasksGroupService>(), tasksGroupService);
            using HttpClient httpClient = testServer.CreateClient();
            HttpResponseMessage response = await httpClient.GetAsync(TasksRoute);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_SuccessStatusCode()
        {
            ITasksGroupService fakeTasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => fakeTasksGroupService.UpdateAsync(A<string>.Ignored, A<string>.Ignored))
                .Returns(new SuccessResponse<ITasksGroup>(A.Fake<ITasksGroup>()));

            using TestServer testServer = CreateTestServerWithFakes(fakeTasksGroupService, A.Fake<IWorkTaskService>());
            using HttpClient httpClient = testServer.CreateClient();

            SaveTasksGroupResource groupResource = new SaveTasksGroupResource { GroupName = "newGroupName" };
            StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(groupResource), Encoding.UTF8, PostMediaType);
            HttpResponseMessage response = await httpClient.PostAsync($"{MainRoute}/some-id", jsonContent);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_AlreadyExistingGroupName_MethodNotAllowedStatusCode()
        {
            string realGroupId = "1001";
            string alreadyExistingGroupName = "Free";

            using TestServer testServer = CreateTestServer();
            using HttpClient httpClient = testServer.CreateClient();

            SaveTasksGroupResource groupResource = new SaveTasksGroupResource { GroupName = alreadyExistingGroupName };
            StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(groupResource), Encoding.UTF8, PostMediaType);
            HttpResponseMessage response = await httpClient.PostAsync($"{MainRoute}/{realGroupId}", jsonContent);

            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task PostAsync_ThrowsException_InternalServerErrorStatusCode()
        {
            ITasksGroupService fakeTasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => fakeTasksGroupService.UpdateAsync(A<string>.Ignored, A<string>.Ignored))
                .Throws<Exception>();

            using TestServer testServer = CreateTestServerWithFakes(fakeTasksGroupService, A.Fake<IWorkTaskService>());
            using HttpClient httpClient = testServer.CreateClient();

            SaveTasksGroupResource groupResource = new SaveTasksGroupResource { GroupName = "newGroupName" };
            StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(groupResource), Encoding.UTF8, PostMediaType);
            HttpResponseMessage response = await httpClient.PostAsync($"{MainRoute}/some-id", jsonContent);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Theory]
        [InlineData("1000")]
        public async Task RemoveGroupAsync_GroupExistAndHasSizeGreaterThanOne_GroupNotRemoved(string id)
        {
            using TestServer testServer = CreateTestServer();
            using HttpClient httpClient = testServer.CreateClient();
            HttpResponseMessage response = await httpClient.DeleteAsync($"{MainRoute}/{id}");

            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task RemoveGroupAsync_RemoveSuccess_SuccessStatusCode()
        {
            ITasksGroupService fakeTasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => fakeTasksGroupService.RemoveAsync(A<string>.Ignored))
                .Returns(new SuccessResponse<ITasksGroup>(A.Fake<ITasksGroup>(), ""));

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