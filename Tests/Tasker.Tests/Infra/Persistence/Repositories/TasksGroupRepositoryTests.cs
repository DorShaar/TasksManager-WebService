using Database.JsonService;
using FakeItEasy;
using Logger.Contracts;
using Microsoft.Extensions.Options;
using ObjectSerializer.Contracts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Takser.App.Persistence.Context;
using Takser.Infra.Options;
using TaskData;
using TaskData.Contracts;
using Tasker.Infra.Persistence.Context;
using Tasker.Infra.Persistence.Repositories;
using Xunit;

namespace Tasker.Tests.Infra.Persistence.Repositories
{
    public class TasksGroupRepositoryTests
    {
        private const string TestFilesDirectory = "TestFiles";
        private readonly string mAlternateDatabasePath = Path.Combine("TestFiles", "tasks_other.db");
        private readonly ITasksGroupBuilder mTasksGroupBuilder = new TaskGroupBuilder();

        [Fact]
        public async Task AddAsync_TaskGroupNotExist_Success()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());

            Assert.False((await tasksGroupRepository.ListAsync()).Any());

            await tasksGroupRepository.AddAsync(tasksGroup);

            Assert.NotNull(await tasksGroupRepository.FindAsync(tasksGroup.ID));
            Assert.Single(await tasksGroupRepository.ListAsync());
        }

        [Fact]
        public async Task AddAsync_TaskGroupAlreadyExist_NotAdded()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());

            Assert.False((await tasksGroupRepository.ListAsync()).Any());

            // Make tasksGroup be already exist.
            database.Entities.Add(tasksGroup);

            await tasksGroupRepository.AddAsync(tasksGroup);

            Assert.NotNull(await tasksGroupRepository.FindAsync(tasksGroup.ID));
            Assert.Single(await tasksGroupRepository.ListAsync());
        }

        [Fact]
        public async Task AddAsync_TaskGroupNameAlreadyExist_NotAdded()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
            ITasksGroup tasksGroupWithSameName = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());

            Assert.NotEqual(tasksGroup.ID, tasksGroupWithSameName.ID);

            Assert.False((await tasksGroupRepository.ListAsync()).Any());

            await tasksGroupRepository.AddAsync(tasksGroup);
            await tasksGroupRepository.AddAsync(tasksGroupWithSameName);

            Assert.NotNull(await tasksGroupRepository.FindAsync(tasksGroup.ID));
            Assert.Null(await tasksGroupRepository.FindAsync(tasksGroupWithSameName.ID));
            Assert.Single(await tasksGroupRepository.ListAsync());
        }

        [Fact]
        public async Task AddAsync_DatabaseIsLoadedAndSavedOnce()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            await tasksGroupRepository.AddAsync(A.Fake<ITasksGroup>());

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task FindAsync_IdExist_Found()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());

            database.Entities.Add(tasksGroup);

            Assert.NotNull(await tasksGroupRepository.FindAsync(tasksGroup.ID));
        }

        [Fact]
        public async Task FindAsync_IdNotExist_NotFound()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            Assert.Null(await tasksGroupRepository.FindAsync("1005"));
        }

        [Fact]
        public async Task FindAsync_DatabaseIsLoadedOnceAndNoteSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            await tasksGroupRepository.FindAsync("group1");

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }

        [Fact]
        public async Task ListAsync_AsExpected()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup1 = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
            ITasksGroup tasksGroup2 = mTasksGroupBuilder.Create("group2", A.Fake<ILogger>());

            database.Entities.Add(tasksGroup1);
            database.Entities.Add(tasksGroup2);

            List<ITasksGroup> tasksGroups = (await tasksGroupRepository.ListAsync()).ToList();
            Assert.Equal(tasksGroup1, tasksGroups[0]);
            Assert.Equal(tasksGroup2, tasksGroups[1]);
        }

        [Fact]
        public async Task ListAsync_HasNoGroups_ReturnsEmptyList()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            Assert.Empty(await tasksGroupRepository.ListAsync());
        }

        [Fact]
        public async Task ListAsync_DatabaseIsLoadedOnceAndNoteSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            await tasksGroupRepository.ListAsync();

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }

        [Fact]
        public async Task RemoveAsync_GroupExists_GroupRemoved()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup1 = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());

            database.Entities.Add(tasksGroup1);

            Assert.Single(await tasksGroupRepository.ListAsync());

            await tasksGroupRepository.RemoveAsync(tasksGroup1);

            Assert.Empty(await tasksGroupRepository.ListAsync());
        }

        [Fact]
        public async Task RemoveAsync_GroupNotExists_RemoveNotPerformed()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup1 = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
            ITasksGroup tasksGroup2 = mTasksGroupBuilder.Create("group2", A.Fake<ILogger>());

            database.Entities.Add(tasksGroup1);

            Assert.Single(await tasksGroupRepository.ListAsync());

            await tasksGroupRepository.RemoveAsync(tasksGroup2);

            Assert.Single(await tasksGroupRepository.ListAsync());
        }

        [Fact]
        public async Task RemoveAsync_GroupNotExists_DatabaseIsLoadedOnceAndNotSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            await tasksGroupRepository.RemoveAsync(A.Fake<ITasksGroup>());

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }

        [Fact]
        public async Task RemoveAsync_GroupExists_DatabaseIsLoadedAndSavedOnce()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
            database.Entities.Add(tasksGroup);

            await tasksGroupRepository.RemoveAsync(tasksGroup);

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateAsync_GroupExists_GroupUpdated()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());

            database.Entities.Add(tasksGroup);

            Assert.Single(await tasksGroupRepository.ListAsync());

            ITasksGroup tasksGroupWithUpdate = await tasksGroupRepository.FindAsync(tasksGroup.ID);

            string newGroupName = "group1_changed";

            tasksGroupWithUpdate.Name = newGroupName;

            await tasksGroupRepository.UpdateAsync(tasksGroupWithUpdate);

            Assert.Single(await tasksGroupRepository.ListAsync());

            ITasksGroup updatedTasksGroup = await tasksGroupRepository.FindAsync(tasksGroup.ID);

            Assert.Equal(newGroupName, updatedTasksGroup.Name);
        }

        [Fact]
        public async Task UpdateAsync_GroupNotExists_GroupNotAdded()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());

            await tasksGroupRepository.UpdateAsync(tasksGroup);

            Assert.Empty(await tasksGroupRepository.ListAsync());
        }

        [Fact]
        public async Task UpdateAsync_GroupNotExists_DatabaseIsLoadedOnceAndNotSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            await tasksGroupRepository.UpdateAsync(A.Fake<ITasksGroup>());

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateAsync_GroupNotExistsDatabaseIsLoadedAndSavedOnce()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
            database.Entities.Add(tasksGroup);

            await tasksGroupRepository.UpdateAsync(tasksGroup);

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustHaveHappenedOnceExactly();
        }
    }
}