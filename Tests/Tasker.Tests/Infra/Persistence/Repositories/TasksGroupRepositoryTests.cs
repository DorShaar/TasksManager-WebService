using Database.JsonService;
using FakeItEasy;
using Logger.Contracts;
using Microsoft.Extensions.Options;
using ObjectSerializer.Contracts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database);

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());

            Assert.False((await tasksGroupRepository.ListAsync()).Any());

            await tasksGroupRepository.AddAsync(tasksGroup);

            Assert.NotNull(await tasksGroupRepository.FindByIdAsync(tasksGroup.ID));
            Assert.Single(await tasksGroupRepository.ListAsync());
        }

        [Fact]
        public async Task AddAsync_TaskGroupAlreadyExist_NotAdded()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database);

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());

            Assert.False((await tasksGroupRepository.ListAsync()).Any());

            // Make tasksGroup be already exist.
            database.Entities.Add(tasksGroup);

            await tasksGroupRepository.AddAsync(tasksGroup);

            Assert.NotNull(await tasksGroupRepository.FindByIdAsync(tasksGroup.ID));
            Assert.Single(await tasksGroupRepository.ListAsync());
        }

        [Fact]
        public async Task AddAsync_TaskGroupNameAlreadyExist_NotAdded()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database);

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
            ITasksGroup tasksGroupWithSameName = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());

            Assert.NotEqual(tasksGroup.ID, tasksGroupWithSameName.ID);

            Assert.False((await tasksGroupRepository.ListAsync()).Any());

            await tasksGroupRepository.AddAsync(tasksGroup);
            await tasksGroupRepository.AddAsync(tasksGroupWithSameName);

            Assert.NotNull(await tasksGroupRepository.FindByIdAsync(tasksGroup.ID));
            Assert.Null(await tasksGroupRepository.FindByIdAsync(tasksGroupWithSameName.ID));
            Assert.Single(await tasksGroupRepository.ListAsync());
        }

        [Fact]
        public async Task FindByIdAsync_IdExist_Found()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database);

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());

            database.Entities.Add(tasksGroup);

            Assert.NotNull(await tasksGroupRepository.FindByIdAsync(tasksGroup.ID));
        }

        [Fact]
        public async Task FindByIdAsync_IdNotExist_NotFound()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database);

            Assert.Null(await tasksGroupRepository.FindByIdAsync("1005"));
        }

        [Fact]
        public async Task ListAsync_AsExpected()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database);

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

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database);

            Assert.Empty(await tasksGroupRepository.ListAsync());
        }

        [Fact]
        public async Task ListAsync_DatabaseChangedAfterInitialize_ReturnCorrectList()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                DatabaseDirectoryPath = TestFilesDirectory
            });

            AppDbContext database = new AppDbContext(databaseOptions, new JsonSerializerWrapper(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database);

            Assert.Equal(17, (await tasksGroupRepository.ListAsync()).Count());

            File.Copy(mAlternateDatabasePath, database.DatabaseFilePath);

            Assert.Equal(15, (await tasksGroupRepository.ListAsync()).Count());
        }

        [Fact]
        public async Task RemoveAsync_GroupExist_GroupRemovedAndWorkTasksMoveToFreeGroup()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database);

            ITasksGroup tasksGroup1 = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
            ITasksGroup tasksGroup2 = mTasksGroupBuilder.Create("group2", A.Fake<ILogger>());

            database.Entities.Add(tasksGroup1);
            database.Entities.Add(tasksGroup2);

            List<ITasksGroup> tasksGroups = (await tasksGroupRepository.RemoveAsync()).ToList();
            Assert.Equal(tasksGroup1, tasksGroups[0]);
            Assert.Equal(tasksGroup2, tasksGroups[1]);
        }

        [Fact]
        public async Task RemoveAsync_GroupNotExist_RemoveNotPerformed()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database);

            ITasksGroup tasksGroup1 = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
            ITasksGroup tasksGroup2 = mTasksGroupBuilder.Create("group2", A.Fake<ILogger>());

            database.Entities.Add(tasksGroup1);
            database.Entities.Add(tasksGroup2);

            List<ITasksGroup> tasksGroups = (await tasksGroupRepository.RemoveAsync()).ToList();
            Assert.Equal(tasksGroup1, tasksGroups[0]);
            Assert.Equal(tasksGroup2, tasksGroups[1]);
        }

        [Fact]
        public void UpdateAsync_GroupExist_GroupUpdated()
        {
            Database database = CreateTestsDatabase();
            ITasksGroup taskGroup = database.GetEntity("A");
            Assert.AreEqual(taskGroup.GetAllTasks().Count(), 0);

            taskGroup.AddTask(new WorkTask("some group", "todo A1", mLogger));
            taskGroup.AddTask(new WorkTask("some group", "todo A2", mLogger));
            database.Update(taskGroup);
            ITasksGroup updatedTaskGroup = database.GetEntity("A");
            Assert.AreEqual(updatedTaskGroup.GetAllTasks().Count(), 2);
        }

        [Fact]
        public void UpdateAsync_GroupNotExist_GroupNotAdded()
        {
            Database database = CreateTestsDatabase();
            ITasksGroup taskGroup = database.GetEntity("A");
            Assert.AreEqual(taskGroup.GetAllTasks().Count(), 0);

            taskGroup.AddTask(new WorkTask("some group", "todo A1", mLogger));
            taskGroup.AddTask(new WorkTask("some group", "todo A2", mLogger));
            database.Update(taskGroup);
            ITasksGroup updatedTaskGroup = database.GetEntity("A");
            Assert.AreEqual(updatedTaskGroup.GetAllTasks().Count(), 2);
        }
    }
}