using Castle.Core.Logging;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using ObjectSerializer.JsonService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskData;
using TaskData.OperationResults;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
using Tasker.App.Persistence.Repositories;
using Tasker.App.Resources;
using Tasker.Domain.Communication;
using Tasker.Infra.Services;
using Xunit;

namespace Tasker.Tests.Infra.Services
{
    public class TasksGroupServiceTests
    {
        private const string mInvalidGroupName = "InvalidGroupNameInvalidGroupNameInvalidGroupNameInvalidGroupNameInvalidGroupName";
        private const string mInvalidTaskName = "InvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskName" +
            "InvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskName";
        private readonly ITasksGroupFactory mTasksGroupFactory;

        public TasksGroupServiceTests()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.UseTaskerDataEntities();
            serviceCollection.UseJsonObjectSerializer();
            ServiceProvider serviceProvider = serviceCollection
                .AddLogging()
                .BuildServiceProvider();

            mTasksGroupFactory = serviceProvider.GetRequiredService<ITasksGroupFactory>();
        }

        [Fact]
        public async Task ListAsync_EmptyDatabase_NullReturned()
        {
            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns<IEnumerable<ITasksGroup>>(null);
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            Assert.Empty(await tasksGroupService.ListAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task ListAsync_EmptyDatabase_EmptyListReturned()
        {
            List<ITasksGroup> expectedList = new List<ITasksGroup>();

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(expectedList);
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            Assert.Empty(await tasksGroupService.ListAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task ListAsync_NonEmptyDatabase_ExpectedListReturned()
        {
            List<ITasksGroup> expectedList = new List<ITasksGroup>
            {
                mTasksGroupFactory.CreateGroup("group1"),
                mTasksGroupFactory.CreateGroup("group2")
            };

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(expectedList);

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            List<ITasksGroup> result = (await tasksGroupService.ListAsync().ConfigureAwait(false)).ToList();
            Assert.Equal(2, result.Count);
            Assert.Equal("group1", result[0].Name);
            Assert.Equal("group2", result[1].Name);
        }

        [Fact]
        public async Task RemoveAsync_IdNotExists_FailResponseReturned()
        {
            const string wrongID = "wrongID";

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(wrongID)).Returns<ITasksGroup>(null);
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<ITasksGroup> response = await tasksGroupService.RemoveAsync(wrongID).ConfigureAwait(false);
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task RemoveAsync_IdNotExists_RemoveNotPerformed()
        {
            const string wrongID = "wrongID";

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(wrongID)).Returns<ITasksGroup>(null);
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            await tasksGroupService.RemoveAsync(wrongID).ConfigureAwait(false);

            A.CallTo(() => dbRepository.RemoveAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task RemoveAsync_GroupWithChildren_FailResponseReturned()
        {
            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("group");
            mTasksGroupFactory.CreateTask(tasksGroup, "task1");

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<ITasksGroup> response = await tasksGroupService.RemoveAsync(tasksGroup.ID).ConfigureAwait(false);
            Assert.False(response.IsSuccess);
            Assert.Equal(tasksGroup, response.ResponseObject);
        }

        [Fact]
        public async Task RemoveAsync_GroupWithChildren_RemoveNotPerformed()
        {
            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("group");
            mTasksGroupFactory.CreateTask(tasksGroup, "task1");

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            await tasksGroupService.RemoveAsync(tasksGroup.ID).ConfigureAwait(false);

            A.CallTo(() => dbRepository.RemoveAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task RemoveAsync_EmptyGroup_SuccessResponseReturned()
        {
            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("emptyGroup");

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<ITasksGroup> response = await tasksGroupService.RemoveAsync(tasksGroup.ID).ConfigureAwait(false);
            Assert.True(response.IsSuccess);
            Assert.Equal(tasksGroup, response.ResponseObject);
        }

        [Fact]
        public async Task RemoveAsync_EmptyGroup_RemovePerformed()
        {
            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("emptyGroup");

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            await tasksGroupService.RemoveAsync(tasksGroup.ID).ConfigureAwait(false);

            A.CallTo(() => dbRepository.RemoveAsync(A<ITasksGroup>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RemoveTaskAsync_TaskExists_SuccessResponseReturned()
        {
            IWorkTask taskToRemove = A.Fake<IWorkTask>();
            OperationResult<IWorkTask> getTaskResult = new OperationResult<IWorkTask>(true, taskToRemove);

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            A.CallTo(() => tasksGroup.GetTask(A<string>.Ignored)).Returns(getTaskResult);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<IWorkTask> response = await tasksGroupService.RemoveTaskAsync(tasksGroup.ID).ConfigureAwait(false);
            Assert.True(response.IsSuccess);
            Assert.Equal(taskToRemove, response.ResponseObject);
        }

        [Fact]
        public async Task RemoveTaskAsync_TaskExists_TaskRemoved()
        {
            IWorkTask taskToRemove = A.Fake<IWorkTask>();
            OperationResult<IWorkTask> getTaskResult = new OperationResult<IWorkTask>(true, taskToRemove);

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            A.CallTo(() => tasksGroup.GetTask(A<string>.Ignored)).Returns(getTaskResult);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            await tasksGroupService.RemoveTaskAsync(tasksGroup.ID).ConfigureAwait(false);

            A.CallTo(() => tasksGroup.RemoveTask(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => dbRepository.UpdateAsync(tasksGroup)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RemoveTaskAsync_WorkTaskNotExists_FailResponseReturned()
        {
            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>());
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<IWorkTask> response = await tasksGroupService.RemoveTaskAsync("notExistingId").ConfigureAwait(false);
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task RemoveTaskAsync_WorkTaskNotExists_RemoveNotPerformed()
        {
            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>());
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<IWorkTask> response = await tasksGroupService.RemoveTaskAsync("notExistingId").ConfigureAwait(false);
            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateTaskAsync_TaskExists_SuccessResponseReturned()
        {
            IWorkTask taskToUpdate = A.Fake<IWorkTask>();
            A.CallTo(() => taskToUpdate.Description).Returns("some-description");
            A.CallTo(() => taskToUpdate.ID).Returns("some-id");

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            OperationResult<IWorkTask> getTaskResult = new OperationResult<IWorkTask>(true, taskToUpdate);
            A.CallTo(() => tasksGroup.GetTask(A<string>.Ignored)).Returns(getTaskResult);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<IWorkTask> response = await tasksGroupService.UpdateTaskAsync(
                new WorkTaskResource
                {
                    TaskId = taskToUpdate.ID,
                    Description = taskToUpdate.Description
                }).ConfigureAwait(false);

            Assert.True(response.IsSuccess);
            Assert.Equal(taskToUpdate, response.ResponseObject);
        }

        [Fact]
        public async Task UpdateTaskAsync_TaskExists_TaskUpdated()
        {
            IWorkTask taskToUpdate = A.Fake<IWorkTask>();
            A.CallTo(() => taskToUpdate.Description).Returns("some-description");
            A.CallTo(() => taskToUpdate.ID).Returns("some-id");

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            OperationResult<IWorkTask> getTaskResult = new OperationResult<IWorkTask>(true, taskToUpdate);
            A.CallTo(() => tasksGroup.GetTask(A<string>.Ignored)).Returns(getTaskResult);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            await tasksGroupService.UpdateTaskAsync(
                new WorkTaskResource
                {
                    TaskId = taskToUpdate.ID,
                    Description = taskToUpdate.Description
                }).ConfigureAwait(false);

            A.CallTo(() => dbRepository.UpdateAsync(tasksGroup)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateTaskAsync_WorkTaskNotExists_FailResponseReturned()
        {
            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>());

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<IWorkTask> response = await tasksGroupService.UpdateTaskAsync(
                new WorkTaskResource
                {
                    TaskId = "notExistingId",
                    Description = "some-description"
                }).ConfigureAwait(false);

            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task UpdateTaskAsync_WorkTaskNotExists_UpdateNotPerformed()
        {
            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>());

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<IWorkTask> response = await tasksGroupService.UpdateTaskAsync(
                new WorkTaskResource
                {
                    TaskId = "notExistingId",
                    Description = "some-description"
                }).ConfigureAwait(false);

            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateTaskAsync_DescriptionAlreadyExist_FailResponseReturnedAndUpdateNotPerformed()
        {
            IWorkTask taskToUpdate = A.Fake<IWorkTask>();
            A.CallTo(() => taskToUpdate.Description).Returns("same-description");
            A.CallTo(() => taskToUpdate.ID).Returns("some-id");

            IWorkTask taskWithSameDescription = A.Fake<IWorkTask>();
            A.CallTo(() => taskWithSameDescription.Description).Returns("same-description");

            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("group-name");
            mTasksGroupFactory.CreateTask(tasksGroup, "same-description");
            mTasksGroupFactory.CreateTask(tasksGroup, "same-description");

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<IWorkTask> response = await tasksGroupService.UpdateTaskAsync(
                new WorkTaskResource
                {
                    TaskId = taskToUpdate.ID,
                    Description = taskWithSameDescription.Description
                }).ConfigureAwait(false);

            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);

            A.CallTo(() => dbRepository.UpdateAsync(tasksGroup)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SaveAsync_InvalidGroupName_SaveNotPerformed()
        {
            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup(mInvalidGroupName);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            await tasksGroupService.SaveAsync(tasksGroup.Name).ConfigureAwait(false);

            A.CallTo(() => dbRepository.AddAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SaveAsync_ValidTasksGroupToAdd_SuccessResponseReturned()
        {
            const string groupName = "ValidGroupName";

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.AddAsync(A<ITasksGroup>.Ignored)).Returns(true);

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<ITasksGroup> response = await tasksGroupService.SaveAsync(groupName).ConfigureAwait(false);
            Assert.True(response.IsSuccess);
            Assert.Equal(groupName, response.ResponseObject.Name);
        }

        [Fact]
        public async Task SaveAsync_ValidTasksGroupToAdd_SavePerformed()
        {
            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("ValidGroupName");

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            await tasksGroupService.SaveAsync(tasksGroup.Name).ConfigureAwait(false);

            A.CallTo(() => dbRepository.AddAsync(A<ITasksGroup>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SaveTaskAsync_InvalidTaskName_FailResponseReturnedAndSaveNotPerformed()
        {
            const string groupName = "groupName";

            IWorkTask workTask = A.Fake<IWorkTask>();
            workTask.GroupName = groupName;
            workTask.Description = mInvalidTaskName;
            OperationResult<IWorkTask> createTaskResult = new OperationResult<IWorkTask>(true, workTask);

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            tasksGroup.SetGroupName(groupName);

            A.CallTo(() => tasksGroup.CreateTask(A<string>.Ignored, A<string>.Ignored)).Returns(createTaskResult);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup> { tasksGroup });

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<IWorkTask> response =
                await tasksGroupService.SaveTaskAsync(workTask.GroupName, workTask.Description).ConfigureAwait(false);
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
            A.CallTo(() => dbRepository.AddAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SaveTaskAsync_GroupIdentifierNotExists_FailResponseReturned()
        {
            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>());

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<IWorkTask> response =
                await tasksGroupService.SaveTaskAsync("notExistingGroupIdentifier", mInvalidTaskName).ConfigureAwait(false);
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task SaveTaskAsync_GroupIdentifierNotExists_SaveNotPerformed()
        {
            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>());

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<IWorkTask> response =
                await tasksGroupService.SaveTaskAsync("notExistingGroupIdentifier", mInvalidTaskName).ConfigureAwait(false);

            A.CallTo(() => dbRepository.AddAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SaveTaskAsync_DescriptionExistsInTheSameGroup_FailResponseReturned()
        {
            IWorkTask workTask = A.Fake<IWorkTask>();
            workTask.GroupName = "groupName";
            workTask.Description = "description";

            IWorkTask workTaskWithSameDescription = A.Fake<IWorkTask>();
            workTask.GroupName = workTask.GroupName;
            workTask.Description = workTask.Description;

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            tasksGroup.SetGroupName(workTask.GroupName);
            A.CallTo(() => tasksGroup.GetAllTasks()).Returns(new List<IWorkTask> { workTask });

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<IWorkTask> response =
                await tasksGroupService.SaveTaskAsync(workTask.GroupName, workTask.Description).ConfigureAwait(false);
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task SaveTaskAsync_DescriptionExistsInTheSameGroup_SaveNotPerformed()
        {
            IWorkTask workTask = A.Fake<IWorkTask>();
            workTask.GroupName = "groupName";
            workTask.Description = "description";

            IWorkTask workTaskWithSameDescription = A.Fake<IWorkTask>();
            workTask.GroupName = workTask.GroupName;
            workTask.Description = workTask.Description;

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            tasksGroup.SetGroupName(workTask.GroupName);
            A.CallTo(() => tasksGroup.GetAllTasks()).Returns(new List<IWorkTask> { workTask });

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            await tasksGroupService.SaveTaskAsync(workTask.GroupName, workTask.Description).ConfigureAwait(false);
            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SaveTaskAsync_ValidWorkTaskToAdd_SuccessResponseReturnedAndSavePerformed()
        {
            IWorkTask validWorkTask = A.Fake<IWorkTask>();
            validWorkTask.GroupName = "groupName";
            validWorkTask.Description = "validDescription";

            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("bla");
            tasksGroup.SetGroupName(validWorkTask.GroupName);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });
            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<IWorkTask> response =
                await tasksGroupService.SaveTaskAsync(validWorkTask.GroupName, validWorkTask.Description)
                .ConfigureAwait(false);

            Assert.True(response.IsSuccess);
            Assert.Equal(tasksGroup.GetAllTasks().First(), response.ResponseObject);
            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateAsync_IdNotExists_FailResponseReturned()
        {
            const string wrongID = "wrongID";

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(wrongID)).Returns<ITasksGroup>(null);

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<ITasksGroup> response =
                await tasksGroupService.UpdateGroupAsync(wrongID, "newGroupName").ConfigureAwait(false);
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task UpdateAsync_IdNotExists_UpdateNotPerformed()
        {
            const string wrongID = "wrongID";

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(wrongID)).Returns<ITasksGroup>(null);

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            await tasksGroupService.UpdateGroupAsync(wrongID, "newGroupName").ConfigureAwait(false);

            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateAsync_GroupWithInvalidName_FailResponseReturned()
        {
            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("group");

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<ITasksGroup> response =
                await tasksGroupService.UpdateGroupAsync(tasksGroup.ID, mInvalidGroupName).ConfigureAwait(false);
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task UpdateAsync_GroupWithInvalidName_UpdateNotPerformed()
        {
            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("group");

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            await tasksGroupService.UpdateGroupAsync(tasksGroup.ID, mInvalidGroupName).ConfigureAwait(false);

            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateAsync_GroupAlreadyExistingWithNewName_FailResponseReturned()
        {
            const string groupName = "groupName";
            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup(groupName);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup> { tasksGroup });

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<ITasksGroup> response =
                await tasksGroupService.UpdateGroupAsync(tasksGroup.ID, groupName).ConfigureAwait(false);
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task UpdateAsync_GroupAlreadyExistingWithNewName_UpdateNotPerformed()
        {
            const string groupName = "groupName";
            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup(groupName);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup> { tasksGroup });

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<ITasksGroup> response =
                await tasksGroupService.UpdateGroupAsync(tasksGroup.ID, groupName).ConfigureAwait(false);

            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateAsync_EmptyGroup_SuccessResponseReturned()
        {
            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("emptyGroup");

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            IResponse<ITasksGroup> response =
                await tasksGroupService.UpdateGroupAsync(tasksGroup.ID, "newGroupName").ConfigureAwait(false);
            Assert.True(response.IsSuccess);
            Assert.Equal(tasksGroup, response.ResponseObject);
        }

        [Fact]
        public async Task UpdateAsync_EmptyGroup_UpdatePerformed()
        {
            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("emptyGroup");

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            await tasksGroupService.UpdateGroupAsync(tasksGroup.ID, "newGroupName").ConfigureAwait(false);

            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateAsync_GroupWithChildren_ValidNewGrupNameGiven_ChildrenUpdatedWithNewGroupName()
        {
            const string oldGroupName = "oldGroupName";
            const string newGroupName = "newGroupName";

            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup(oldGroupName);
            IWorkTask workTask1 = mTasksGroupFactory.CreateTask(tasksGroup, "workTask1");
            IWorkTask workTask2 = mTasksGroupFactory.CreateTask(tasksGroup, "workTask2");

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);

            TasksGroupService tasksGroupService =
                new TasksGroupService(dbRepository, mTasksGroupFactory, NullLogger<TasksGroupService>.Instance);

            await tasksGroupService.UpdateGroupAsync(tasksGroup.ID, newGroupName).ConfigureAwait(false);

            Assert.Equal(newGroupName, workTask1.GroupName);
            Assert.Equal(newGroupName, workTask2.GroupName);
        }
    }
}