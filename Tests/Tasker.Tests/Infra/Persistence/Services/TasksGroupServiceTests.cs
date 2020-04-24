using FakeItEasy;
using Logger.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskData;
using TaskData.Contracts;
using Tasker.App.Persistence.Repositories;
using Tasker.App.Resources;
using Tasker.Domain.Communication;
using Tasker.Infra.Services;
using Xunit;

namespace Tasker.Tests.Infra.Persistence.Services
{
    public class TasksGroupServiceTests
    {
        private const string mInvalidGroupName = "InvalidGroupNameInvalidGroupNameInvalidGroupNameInvalidGroupNameInvalidGroupName";
        private const string mInvalidTaskName = "InvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskName" +
            "InvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskNameInvalidTaskName";
        private readonly ITasksGroupBuilder mTasksGroupBuilder = new TaskGroupBuilder();

        [Fact]
        public async Task ListAsync_EmptyDatabase_NullReturned()
        {
            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns<IEnumerable<ITasksGroup>>(null);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            Assert.Empty(await tasksGroupService.ListAsync());
        }

        [Fact]
        public async Task ListAsync_EmptyDatabase_EmptyListReturned()
        {
            List<ITasksGroup> expectedList = new List<ITasksGroup>();

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(expectedList);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            Assert.Empty(await tasksGroupService.ListAsync());
        }

        [Fact]
        public async Task ListAsync_NonEmptyDatabase_ExpectedListReturned()
        {
            List<ITasksGroup> expectedList = new List<ITasksGroup>
            {
                mTasksGroupBuilder.Create("group1", A.Fake<ILogger>()),
                mTasksGroupBuilder.Create("group2", A.Fake<ILogger>())
            };

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(expectedList);

            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            List<ITasksGroup> result = (await tasksGroupService.ListAsync()).ToList();
            Assert.Equal(2, result.Count);
            Assert.Equal("group1", result[0].Name);
            Assert.Equal("group2", result[1].Name);
        }

        [Fact]
        public async Task RemoveAsync_IdNotExists_FailResponseReturned()
        {
            string wrongID = "wrongID";

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(wrongID)).Returns<ITasksGroup>(null);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<ITasksGroup> response = await tasksGroupService.RemoveAsync(wrongID);
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task RemoveAsync_IdNotExists_RemoveNotPerformed()
        {
            string wrongID = "wrongID";

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(wrongID)).Returns<ITasksGroup>(null);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.RemoveAsync(wrongID);

            A.CallTo(() => dbRepository.RemoveAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task RemoveAsync_GroupWithChildren_FailResponseReturned()
        {
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group", A.Fake<ILogger>());
            tasksGroup.CreateTask("task1");

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<ITasksGroup> response = await tasksGroupService.RemoveAsync(tasksGroup.ID);
            Assert.False(response.IsSuccess);
            Assert.Equal(tasksGroup, response.ResponseObject);
        }

        [Fact]
        public async Task RemoveAsync_GroupWithChildren_RemoveNotPerformed()
        {
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group", A.Fake<ILogger>());
            tasksGroup.CreateTask("task1");

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.RemoveAsync(tasksGroup.ID);

            A.CallTo(() => dbRepository.RemoveAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task RemoveAsync_EmptyGroup_SuccessResponseReturned()
        {
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("emptyGroup", A.Fake<ILogger>());

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<ITasksGroup> response = await tasksGroupService.RemoveAsync(tasksGroup.ID);
            Assert.True(response.IsSuccess);
            Assert.Equal(tasksGroup, response.ResponseObject);
        }

        [Fact]
        public async Task RemoveAsync_EmptyGroup_RemovePerformed()
        {
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("emptyGroup", A.Fake<ILogger>());

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.RemoveAsync(tasksGroup.ID);

            A.CallTo(() => dbRepository.RemoveAsync(A<ITasksGroup>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RemoveTaskAsync_TaskExists_SuccessResponseReturned()
        {
            IWorkTask taskToRemove = A.Fake<IWorkTask>();

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            A.CallTo(() => tasksGroup.GetTask(A<string>.Ignored)).Returns(taskToRemove);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<IWorkTask> response = await tasksGroupService.RemoveTaskAsync(tasksGroup.ID);
            Assert.True(response.IsSuccess);
            Assert.Equal(taskToRemove, response.ResponseObject);
        }

        [Fact]
        public async Task RemoveTaskAsync_TaskExists_TaskRemoved()
        {
            IWorkTask taskToRemove = A.Fake<IWorkTask>();

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            A.CallTo(() => tasksGroup.GetTask(A<string>.Ignored)).Returns(taskToRemove);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.RemoveTaskAsync(tasksGroup.ID);

            A.CallTo(() => tasksGroup.RemoveTask(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => dbRepository.UpdateAsync(tasksGroup)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RemoveTaskAsync_WorkTaskNotExists_FailResponseReturned()
        {
            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>());
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<IWorkTask> response = await tasksGroupService.RemoveTaskAsync("notExistingId");
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task RemoveTaskAsync_WorkTaskNotExists_RemoveNotPerformed()
        {
            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>());
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<IWorkTask> response = await tasksGroupService.RemoveTaskAsync("notExistingId");
            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateTaskAsync_TaskExists_SuccessResponseReturned()
        {
            IWorkTask taskToUpdate = A.Fake<IWorkTask>();
            A.CallTo(() => taskToUpdate.Description).Returns("some-description");
            A.CallTo(() => taskToUpdate.ID).Returns("some-id");

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            A.CallTo(() => tasksGroup.GetTask(A<string>.Ignored)).Returns(taskToUpdate);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<IWorkTask> response = await tasksGroupService.UpdateTaskAsync(
                new WorkTaskResource
                {
                    TaskId = taskToUpdate.ID,
                    Description = taskToUpdate.Description
                });

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
            A.CallTo(() => tasksGroup.GetTask(A<string>.Ignored)).Returns(taskToUpdate);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.UpdateTaskAsync(
                new WorkTaskResource
                {
                    TaskId = taskToUpdate.ID,
                    Description = taskToUpdate.Description
                });

            A.CallTo(() => dbRepository.UpdateAsync(tasksGroup)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateTaskAsync_WorkTaskNotExists_FailResponseReturned()
        {
            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>());
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<IWorkTask> response = await tasksGroupService.UpdateTaskAsync(
                new WorkTaskResource
                {
                    TaskId = "notExistingId",
                    Description = "some-description"
                });

            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task UpdateTaskAsync_WorkTaskNotExists_UpdateNotPerformed()
        {
            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>());
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<IWorkTask> response = await tasksGroupService.UpdateTaskAsync(
                new WorkTaskResource
                {
                    TaskId = "notExistingId",
                    Description = "some-description"
                });

            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateTaskAsync_DescriptionAlreadyExist_FailResponseReturned()
        {
            IWorkTask taskToUpdate = A.Fake<IWorkTask>();
            A.CallTo(() => taskToUpdate.Description).Returns("some-description");
            A.CallTo(() => taskToUpdate.ID).Returns("some-id");

            IWorkTask taskWithSameDescription = A.Fake<IWorkTask>();
            A.CallTo(() => taskWithSameDescription.Description).Returns("existing-description");

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            A.CallTo(() => tasksGroup.GetAllTasks()).Returns(new List<IWorkTask> { taskToUpdate, taskWithSameDescription });

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<IWorkTask> response = await tasksGroupService.UpdateTaskAsync(
                new WorkTaskResource
                {
                    TaskId = taskToUpdate.ID,
                    Description = taskWithSameDescription.Description
                });

            A.CallTo(() => tasksGroup.GetAllTasks()).MustHaveHappenedOnceExactly();
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task UpdateTaskAsync_DescriptionAlreadyExist_UpdateNotPerformed()
        {
            IWorkTask taskToUpdate = A.Fake<IWorkTask>();
            A.CallTo(() => taskToUpdate.Description).Returns("some-description");
            A.CallTo(() => taskToUpdate.ID).Returns("some-id");

            IWorkTask taskWithSameDescription= A.Fake<IWorkTask>();
            A.CallTo(() => taskWithSameDescription.Description).Returns("existing-description");

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            A.CallTo(() => tasksGroup.GetAllTasks()).Returns(new List<IWorkTask> { taskToUpdate, taskWithSameDescription });

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.UpdateTaskAsync(
                new WorkTaskResource
                {
                    TaskId = taskToUpdate.ID,
                    Description = taskWithSameDescription.Description
                });

            A.CallTo(() => tasksGroup.GetAllTasks()).MustHaveHappenedOnceExactly();
            A.CallTo(() => dbRepository.UpdateAsync(tasksGroup)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SaveAsync_InvalidGroupName_SaveNotPerformed()
        {
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create(mInvalidGroupName, A.Fake<ILogger>());

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.SaveAsync(tasksGroup.Name);

            A.CallTo(() => dbRepository.AddAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SaveAsync_ValidTasksGroupToAdd_SuccessResponseReturned()
        {
            string groupName = "ValidGroupName";

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<ITasksGroup> response = await tasksGroupService.SaveAsync(groupName);
            Assert.True(response.IsSuccess);
            Assert.Equal(groupName, response.ResponseObject.Name);
        }

        [Fact]
        public async Task SaveAsync_ValidTasksGroupToAdd_SavePerformed()
        {
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("ValidGroupName", A.Fake<ILogger>());

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.SaveAsync(tasksGroup.Name);

            A.CallTo(() => dbRepository.AddAsync(A<ITasksGroup>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SaveTaskAsync_InvalidTaskName_FailResponseReturned()
        {
            string groupName = "groupName";

            IWorkTask workTask = A.Fake<IWorkTask>();
            workTask.GroupName = groupName;
            workTask.Description = mInvalidTaskName;

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            tasksGroup.Name = groupName;

            A.CallTo(() => tasksGroup.CreateTask(A<string>.Ignored)).Returns(workTask);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup> { tasksGroup });

            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<IWorkTask> response = await tasksGroupService.SaveTaskAsync(workTask.GroupName, workTask.Description);
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task SaveTaskAsync_InvalidTaskName_SaveNotPerformed()
        {
            string groupName = "groupName";

            IWorkTask workTask = A.Fake<IWorkTask>();
            workTask.GroupName = groupName;
            workTask.Description = mInvalidTaskName;

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            tasksGroup.Name = groupName;

            A.CallTo(() => tasksGroup.CreateTask(A<string>.Ignored)).Returns(workTask);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup> { tasksGroup });

            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.SaveTaskAsync(workTask.GroupName, workTask.Description);

            A.CallTo(() => dbRepository.AddAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SaveTaskAsync_GroupIdentifierNotExists_FailResponseReturned()
        {
            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>());

            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<IWorkTask> response = await tasksGroupService.SaveTaskAsync("notExistingGroupIdentifier", mInvalidTaskName);
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task SaveTaskAsync_GroupIdentifierNotExists_SaveNotPerformed()
        {
            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>());

            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<IWorkTask> response = await tasksGroupService.SaveTaskAsync("notExistingGroupIdentifier", mInvalidTaskName);

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
            tasksGroup.Name = workTask.GroupName;
            A.CallTo(() => tasksGroup.GetAllTasks()).Returns(new List<IWorkTask> { workTask });

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<IWorkTask> response = await tasksGroupService.SaveTaskAsync(workTask.GroupName, workTask.Description);
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
            tasksGroup.Name = workTask.GroupName;
            A.CallTo(() => tasksGroup.GetAllTasks()).Returns(new List<IWorkTask> { workTask });

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.SaveTaskAsync(workTask.GroupName, workTask.Description);
            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SaveTaskAsync_ValidWorkTaskToAdd_SuccessResponseReturned()
        {
            IWorkTask validWorkTask = A.Fake<IWorkTask>();
            validWorkTask.GroupName = "groupName";
            validWorkTask.Description = "validDescription";

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            tasksGroup.Name = validWorkTask.GroupName;
            A.CallTo(() => tasksGroup.CreateTask(A<string>.Ignored)).Returns(validWorkTask);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<IWorkTask> response = await tasksGroupService.SaveTaskAsync(validWorkTask.GroupName, validWorkTask.Description);
            Assert.True(response.IsSuccess);
            Assert.Equal(validWorkTask, response.ResponseObject);
        }

        [Fact]
        public async Task SaveTaskAsync_ValidWorkTaskToAdd_SavePerformed()
        {
            IWorkTask validWorkTask = A.Fake<IWorkTask>();
            validWorkTask.GroupName = "groupName";
            validWorkTask.Description = "validDescription";

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            tasksGroup.Name = validWorkTask.GroupName;
            A.CallTo(() => tasksGroup.CreateTask(A<string>.Ignored)).Returns(validWorkTask);

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup>() { tasksGroup });
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.SaveTaskAsync(validWorkTask.GroupName, validWorkTask.Description);

            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateAsync_IdNotExists_FailResponseReturned()
        {
            string wrongID = "wrongID";

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(wrongID)).Returns<ITasksGroup>(null);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<ITasksGroup> response = await tasksGroupService.UpdateGroupAsync(wrongID, "newGroupName");
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task UpdateAsync_IdNotExists_UpdateNotPerformed()
        {
            string wrongID = "wrongID";

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(wrongID)).Returns<ITasksGroup>(null);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.UpdateGroupAsync(wrongID, "newGroupName");

            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateAsync_GroupWithInvalidName_FailResponseReturned()
        {
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group", A.Fake<ILogger>());

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<ITasksGroup> response = await tasksGroupService.UpdateGroupAsync(tasksGroup.ID, mInvalidGroupName);
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task UpdateAsync_GroupWithInvalidName_UpdateNotPerformed()
        {
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group", A.Fake<ILogger>());

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.UpdateGroupAsync(tasksGroup.ID, mInvalidGroupName);

            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateAsync_GroupAlreadyExistingWithNewName_FailResponseReturned()
        {
            string groupName = "groupName";
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create(groupName, A.Fake<ILogger>());

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup> { tasksGroup });
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<ITasksGroup> response = await tasksGroupService.UpdateGroupAsync(tasksGroup.ID, groupName);
            Assert.False(response.IsSuccess);
            Assert.Null(response.ResponseObject);
        }

        [Fact]
        public async Task UpdateAsync_GroupAlreadyExistingWithNewName_UpdateNotPerformed()
        {
            string groupName = "groupName";
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create(groupName, A.Fake<ILogger>());

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.ListAsync()).Returns(new List<ITasksGroup> { tasksGroup });
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<ITasksGroup> response = await tasksGroupService.UpdateGroupAsync(tasksGroup.ID, groupName);

            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateAsync_EmptyGroup_SuccessResponseReturned()
        {
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("emptyGroup", A.Fake<ILogger>());

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            IResponse<ITasksGroup> response = await tasksGroupService.UpdateGroupAsync(tasksGroup.ID, "newGroupName");
            Assert.True(response.IsSuccess);
            Assert.Equal(tasksGroup, response.ResponseObject);
        }

        [Fact]
        public async Task UpdateAsync_EmptyGroup_UpdatePerformed()
        {
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("emptyGroup", A.Fake<ILogger>());

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.UpdateGroupAsync(tasksGroup.ID, "newGroupName");

            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateAsync_GroupWithChildren_ValidNewGrupNameGiven_ChildrenUpdatedWithNewGroupName()
        {
            string oldGroupName = "oldGroupName";
            string newGroupName = "newGroupName";

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create(oldGroupName, A.Fake<ILogger>());
            IWorkTask workTask1 = tasksGroup.CreateTask("workTask1");
            IWorkTask workTask2 = tasksGroup.CreateTask("workTask2");

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.UpdateGroupAsync(tasksGroup.ID, newGroupName);

            Assert.Equal(newGroupName, workTask1.GroupName);
            Assert.Equal(newGroupName, workTask2.GroupName);
        }
    }
}