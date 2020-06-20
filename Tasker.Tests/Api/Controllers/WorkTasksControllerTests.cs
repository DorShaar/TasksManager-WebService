using FakeItEasy;
using Microsoft.AspNetCore.TestHost;
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
    public class WorkTasksControllerTests
    {
        private const string MainRoute = "api/WorkTasks";
        private const string PostMediaType = "application/json";

        [Fact]
        public async Task ListTasksOfSpecificGroupAsync_SuccessStatusCode()
        {
            string groupId = "some-id";
            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            A.CallTo(() => tasksGroup.ID).Returns(groupId);

            ITasksGroupService tasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => tasksGroupService.ListAsync()).Returns(new List<ITasksGroup>());

            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes(tasksGroupService);
            using HttpClient httpClient = testServer.CreateClient();
            using HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}/{groupId}").ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ListTasksAsync_NullListReturned_EmptyListReturned()
        {
            List<IWorkTask> tasksList = null;

            IWorkTaskService workTaskService = A.Fake<IWorkTaskService>();
            A.CallTo(() => workTaskService.ListAsync()).Returns(tasksList);

            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes(workTaskService: workTaskService);
            using HttpClient httpClient = testServer.CreateClient();
            using HttpResponseMessage response = await httpClient.GetAsync(MainRoute).ConfigureAwait(false);

            string stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            IEnumerable<WorkTaskResource> workTaskResources =
                JsonConvert.DeserializeObject<IEnumerable<WorkTaskResource>>(stringResponse);

            Assert.Empty(workTaskResources);
        }

        [Fact]
        public async Task ListTasksAsync_SuccessStatusCode()
        {
            List<IWorkTask> tasksList = new List<IWorkTask>();

            IWorkTaskService workTaskService = A.Fake<IWorkTaskService>();
            A.CallTo(() => workTaskService.ListAsync()).Returns(tasksList);

            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes(workTaskService: workTaskService);
            using HttpClient httpClient = testServer.CreateClient();
            using HttpResponseMessage response = await httpClient.GetAsync(MainRoute).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ListTasksOfSpecificGroupAsync_groupIdNullOrEmpty_EmptyListReturned(string groupId)
        {
            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes();
            using HttpClient httpClient = testServer.CreateClient();
            using HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}/{groupId}").ConfigureAwait(false);

            string stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            IEnumerable<WorkTaskResource> workTaskResources =
                JsonConvert.DeserializeObject<IEnumerable<WorkTaskResource>>(stringResponse);

            Assert.Empty(workTaskResources);
        }

        [Fact]
        public async Task ListTasksOfSpecificGroupAsync_groupNotFound_EmptyListReturned()
        {
            ITasksGroupService tasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => tasksGroupService.ListAsync()).Returns(new List<ITasksGroup>());

            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes(tasksGroupService);
            using HttpClient httpClient = testServer.CreateClient();
            using HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}/some-id").ConfigureAwait(false);

            string stringResponse = await response.Content.ReadAsStringAsync();
            IEnumerable<WorkTaskResource> workTaskResources =
                JsonConvert.DeserializeObject<IEnumerable<WorkTaskResource>>(stringResponse);

            Assert.Empty(workTaskResources);
        }

        [Fact]
        public async Task PutWorkTaskAsync_InvalidWorkTaskResourceException_BadRequestReturned()
        {
            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes();
            using HttpClient httpClient = testServer.CreateClient();

            WorkTaskResource workTaskResource = new WorkTaskResource { GroupName = "newGroupName" };
            using StringContent jsonContent = new StringContent("invalidResource", Encoding.UTF8, PostMediaType);
            using HttpResponseMessage response = await httpClient.PutAsync(MainRoute, jsonContent).ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutWorkTaskAsync_RequestNotSuccess_MethodNotAllowedReturned()
        {
            ITasksGroupService taskGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => taskGroupService.SaveTaskAsync(A<string>.Ignored, A<string>.Ignored))
                .Returns(new FailResponse<IWorkTask>(""));

            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes(taskGroupService);
            using HttpClient httpClient = testServer.CreateClient();

            WorkTaskResource workTaskResource = new WorkTaskResource { GroupName = "newGroupName" };
            using StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType);
            using HttpResponseMessage response = await httpClient.PutAsync(MainRoute, jsonContent).ConfigureAwait(false);

            A.CallTo(() => taskGroupService.SaveTaskAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task PutWorkTaskAsync_RequestSuccess_SuccessStatusCode()
        {
            WorkTaskResource workTaskResource = new WorkTaskResource { GroupName = "newGroupName", Description = "description" };

            ITasksGroupService tasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => tasksGroupService.SaveTaskAsync(workTaskResource.GroupName, workTaskResource.Description))
                .Returns(new SuccessResponse<IWorkTask>(A.Fake<IWorkTask>()));

            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes(tasksGroupService);
            using HttpClient httpClient = testServer.CreateClient();
            
            using StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType);
            using HttpResponseMessage response = await httpClient.PutAsync(MainRoute, jsonContent).ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PutWorkTaskAsync_RequestSuccess_ExpectedResourceReturned()
        {
            IWorkTask expectedWorkTask = A.Fake<IWorkTask>();
            A.CallTo(() => expectedWorkTask.Description).Returns("description");
            A.CallTo(() => expectedWorkTask.GroupName).Returns("newGroupName");

            ITasksGroupService tasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => tasksGroupService.SaveTaskAsync(expectedWorkTask.GroupName, expectedWorkTask.Description))
                .Returns(new SuccessResponse<IWorkTask>(expectedWorkTask));

            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes(tasksGroupService);
            using HttpClient httpClient = testServer.CreateClient();

            WorkTaskResource workTaskResource = new WorkTaskResource 
            { 
                GroupName = expectedWorkTask.GroupName, 
                Description = expectedWorkTask.Description 
            };
            using StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType);
            using HttpResponseMessage response = await httpClient.PutAsync(MainRoute, jsonContent).ConfigureAwait(false);

            string stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            WorkTaskResource returnedResource = JsonConvert.DeserializeObject<WorkTaskResource>(stringResponse);

            Assert.Equal(expectedWorkTask.GroupName, returnedResource.GroupName);
            Assert.Equal(expectedWorkTask.Description, returnedResource.Description);
        }

        [Fact]
        public async Task PutWorkTaskAsync_ThrowsException_InternalServerErrorStatusCode()
        {
            ITasksGroupService fakeTasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => fakeTasksGroupService.SaveTaskAsync(A<string>.Ignored, A<string>.Ignored))
                .Throws<Exception>();

            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes(fakeTasksGroupService);
            using HttpClient httpClient = testServer.CreateClient();

            WorkTaskResource workTaskResource = new WorkTaskResource { GroupName = "newGroupName", Description = "description" };
            using StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType);
            using HttpResponseMessage response = await httpClient.PutAsync(MainRoute, jsonContent).ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}