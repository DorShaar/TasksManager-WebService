using FakeItEasy;
using Logger.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskData;
using TaskData.Contracts;
using Tasker.App.Persistence.Repositories;
using Tasker.Domain.Communication;
using Tasker.Infra.Services;
using Xunit;

namespace Tasker.Tests.Infra.Persistence.Services
{
    public class TasksGroupServiceTests
    {
        private const string mInvalidGroupName = "InvalidGroupNameInvalidGroupNameInvalidGroupNameInvalidGroupNameInvalidGroupName";
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

            Response<ITasksGroup> response = await tasksGroupService.RemoveAsync(wrongID);
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

            Response<ITasksGroup> response = await tasksGroupService.RemoveAsync(tasksGroup.ID);
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
        public async Task RemoveAsync_EmptyGroup_SucessResponseReturned()
        {
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("emptyGroup", A.Fake<ILogger>());

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            Response<ITasksGroup> response = await tasksGroupService.RemoveAsync(tasksGroup.ID);
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
        public async Task SaveAsync_InvalidGroupName_SaveNotPerformed()
        {
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create(mInvalidGroupName, A.Fake<ILogger>());

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            await tasksGroupService.SaveAsync(tasksGroup.Name);

            A.CallTo(() => dbRepository.AddAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
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
        public async Task SaveAsync_ValidTasksGroupToAdd_SucessResponseReturned()
        {
            string groupName = "ValidGroupName";

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            Response<ITasksGroup> response = await tasksGroupService.SaveAsync(groupName);
            Assert.True(response.IsSuccess);
            Assert.Equal(groupName, response.ResponseObject.Name);
        }

        [Fact]
        public async Task UpdateAsync_IdNotExists_FailResponseReturned()
        {
            string wrongID = "wrongID";

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(wrongID)).Returns<ITasksGroup>(null);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            Response<ITasksGroup> response = await tasksGroupService.UpdateAsync(wrongID, "newGroupName");
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

            await tasksGroupService.UpdateAsync(wrongID, "newGroupName");

            A.CallTo(() => dbRepository.RemoveAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateAsync_GroupWithInvalidName_FailResponseReturned()
        {
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group", A.Fake<ILogger>());

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            Response<ITasksGroup> response = await tasksGroupService.UpdateAsync(tasksGroup.ID, mInvalidGroupName);
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

            await tasksGroupService.UpdateAsync(tasksGroup.ID, mInvalidGroupName);

            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateAsync_EmptyGroup_SucessResponseReturned()
        {
            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("emptyGroup", A.Fake<ILogger>());

            IDbRepository<ITasksGroup> dbRepository = A.Fake<IDbRepository<ITasksGroup>>();
            A.CallTo(() => dbRepository.FindAsync(tasksGroup.ID)).Returns(tasksGroup);
            TasksGroupService tasksGroupService = new TasksGroupService(dbRepository, mTasksGroupBuilder, A.Fake<ILogger>());

            Response<ITasksGroup> response = await tasksGroupService.UpdateAsync(tasksGroup.ID, "newGroupName");
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

            await tasksGroupService.UpdateAsync(tasksGroup.ID, "newGroupName");

            A.CallTo(() => dbRepository.UpdateAsync(A<ITasksGroup>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}