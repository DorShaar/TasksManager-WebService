using FakeItEasy;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
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
        private const string PostMediaType = "application/json";

        [Fact]
        public async Task ListGroupsAsync_SuccessStatusCode()
        {
            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes();
            using HttpClient httpClient = testServer.CreateClient();
            using HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}").ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostGroupAsync_SuccessStatusCode()
        {
            ITasksGroupService fakeTasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => fakeTasksGroupService.UpdateGroupAsync(A<string>.Ignored, A<string>.Ignored))
                .Returns(new SuccessResponse<ITasksGroup>(A.Fake<ITasksGroup>()));

            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes(fakeTasksGroupService);
            using HttpClient httpClient = testServer.CreateClient();

            TasksGroupResource groupResource = new TasksGroupResource { GroupName = "newGroupName" };
            using StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(groupResource), Encoding.UTF8, PostMediaType);
            using HttpResponseMessage response = await httpClient.PostAsync($"{MainRoute}/some-id", jsonContent).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostGroupAsync_AlreadyExistingGroupName_MethodNotAllowedStatusCode()
        {
            string realGroupId = "1001";
            string alreadyExistingGroupName = "Free";

            using TestServer testServer = ApiTestHelper.CreateTestServer();
            using HttpClient httpClient = testServer.CreateClient();

            TasksGroupResource groupResource = new TasksGroupResource { GroupName = alreadyExistingGroupName };
            using StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(groupResource), Encoding.UTF8, PostMediaType);
            using HttpResponseMessage response = await httpClient.PostAsync($"{MainRoute}/{realGroupId}", jsonContent).ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task PostGroupAsync_ThrowsException_InternalServerErrorStatusCode()
        {
            ITasksGroupService fakeTasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => fakeTasksGroupService.UpdateGroupAsync(A<string>.Ignored, A<string>.Ignored))
                .Throws<Exception>();

            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes(fakeTasksGroupService);
            using HttpClient httpClient = testServer.CreateClient();

            TasksGroupResource groupResource = new TasksGroupResource { GroupName = "newGroupName" };
            using StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(groupResource), Encoding.UTF8, PostMediaType);
            using HttpResponseMessage response = await httpClient.PostAsync($"{MainRoute}/some-id", jsonContent).ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Theory]
        [InlineData("1000")]
        public async Task RemoveGroupAsync_GroupExistAndHasSizeGreaterThanOne_GroupNotRemoved(string id)
        {
            using TestServer testServer = ApiTestHelper.CreateTestServer();
            using HttpClient httpClient = testServer.CreateClient();
            using HttpResponseMessage response = await httpClient.DeleteAsync($"{MainRoute}/{id}").ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task RemoveGroupAsync_RemoveSuccess_SuccessStatusCode()
        {
            ITasksGroupService fakeTasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => fakeTasksGroupService.RemoveAsync(A<string>.Ignored))
                .Returns(new SuccessResponse<ITasksGroup>(A.Fake<ITasksGroup>(), ""));

            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes(fakeTasksGroupService);
            using HttpClient httpClient = testServer.CreateClient();
            using HttpResponseMessage response = await httpClient.DeleteAsync($"{MainRoute}/some-id").ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RemoveGroupAsync_ThrowsException_InternalServerErrorStatusCode()
        {
            ITasksGroupService fakeTasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => fakeTasksGroupService.RemoveAsync(A<string>.Ignored))
                .Throws<Exception>();

            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes(fakeTasksGroupService);
            using HttpClient httpClient = testServer.CreateClient();
            using HttpResponseMessage response = await httpClient.DeleteAsync($"{MainRoute}/some-id").ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}