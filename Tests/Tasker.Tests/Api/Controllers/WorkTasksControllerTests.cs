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

            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakes(tasksGroupService, A.Fake<IWorkTaskService>());
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

            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakes(A.Fake<ITasksGroupService>(), tasksGroupService);
            using HttpClient httpClient = testServer.CreateClient();
            HttpResponseMessage response = await httpClient.GetAsync(MainRoute);

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

            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakes(A.Fake<ITasksGroupService>(), tasksGroupService);
            using HttpClient httpClient = testServer.CreateClient();
            HttpResponseMessage response = await httpClient.GetAsync(MainRoute);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ListTasksOfSpecificGroupAsync_groupIdNullOrEmpty_EmptyListReturned(string groupId)
        {
            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakes(A.Fake<ITasksGroupService>(), A.Fake<IWorkTaskService>());
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

            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakes(tasksGroupService, A.Fake<IWorkTaskService>());
            using HttpClient httpClient = testServer.CreateClient();
            HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}/some-id");

            string stringResponse = await response.Content.ReadAsStringAsync();
            IEnumerable<WorkTaskResource> workTaskResources =
                JsonConvert.DeserializeObject<IEnumerable<WorkTaskResource>>(stringResponse);

            Assert.Empty(workTaskResources);
        }

        [Fact]
        public async Task PutWorkTaskAsync_InvalidWorkTaskResourceException_BadRequestReturned()
        {
            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakes(A.Fake<ITasksGroupService>(), A.Fake<IWorkTaskService>());
            using HttpClient httpClient = testServer.CreateClient();

            WorkTaskResource workTaskResource = new WorkTaskResource { GroupName = "newGroupName" };
            StringContent jsonContent = new StringContent("invalidResource", Encoding.UTF8, PostMediaType);
            HttpResponseMessage response = await httpClient.PutAsync(MainRoute, jsonContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutWorkTaskAsync_RequestNotSuccess_BadRequestReturned()
        {
            ITasksGroupService taskGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => taskGroupService.SaveTaskAsync(A<string>.Ignored, A<string>.Ignored))
                .Returns(new FailResponse<IWorkTask>(""));

            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakes(taskGroupService, A.Fake<IWorkTaskService>());
            using HttpClient httpClient = testServer.CreateClient();

            WorkTaskResource workTaskResource = new WorkTaskResource { GroupName = "newGroupName" };
            StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType);
            HttpResponseMessage response = await httpClient.PutAsync(MainRoute, jsonContent);

            A.CallTo(() => taskGroupService.SaveTaskAsync(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutWorkTaskAsync_RequestSuccess_SuccessStatusCode()
        {
            WorkTaskResource workTaskResource = new WorkTaskResource { GroupName = "newGroupName", Description = "description" };

            ITasksGroupService tasksGroupService = A.Fake<ITasksGroupService>();
            A.CallTo(() => tasksGroupService.SaveTaskAsync(workTaskResource.GroupName, workTaskResource.Description))
                .Returns(new SuccessResponse<IWorkTask>(A.Fake<IWorkTask>()));

            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakes(tasksGroupService, A.Fake<IWorkTaskService>());
            using HttpClient httpClient = testServer.CreateClient();
            
            StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType);
            HttpResponseMessage response = await httpClient.PutAsync(MainRoute, jsonContent);

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

            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakes(tasksGroupService, A.Fake<IWorkTaskService>());
            using HttpClient httpClient = testServer.CreateClient();

            WorkTaskResource workTaskResource = new WorkTaskResource 
            { 
                GroupName = expectedWorkTask.GroupName, 
                Description = expectedWorkTask.Description 
            };
            StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType);
            HttpResponseMessage response = await httpClient.PutAsync(MainRoute, jsonContent);

            string stringResponse = await response.Content.ReadAsStringAsync();
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

            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakes(fakeTasksGroupService, A.Fake<IWorkTaskService>());
            using HttpClient httpClient = testServer.CreateClient();

            WorkTaskResource workTaskResource = new WorkTaskResource { GroupName = "newGroupName", Description = "description" };
            StringContent jsonContent = new StringContent(JsonConvert.SerializeObject(workTaskResource), Encoding.UTF8, PostMediaType);
            HttpResponseMessage response = await httpClient.PutAsync(MainRoute, jsonContent);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}