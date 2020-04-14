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
    public class WorkTaskRepositoryTests
    {
        private const string TestFilesDirectory = "TestFiles";
        private readonly string mAlternateDatabasePath = Path.Combine("TestFiles", "tasks_other.db");
        private readonly ITasksGroupBuilder mTasksGroupBuilder = new TaskGroupBuilder();

        [Fact]
        public async Task AddAsync_WorkTaskNotExist_Success()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            string tasksGroupName = "group1";

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create(tasksGroupName, A.Fake<ILogger>());

            database.Entities.Add(tasksGroup);

            WorkTask workTask = new WorkTask(tasksGroupName, "worktask1", A.Fake<ILogger>());

            Assert.False((await workTaskRepository.ListAsync()).Any());

            await workTaskRepository.AddAsync(workTask);

            Assert.NotNull(await workTaskRepository.FindAsync(workTask.ID));
            Assert.Single(await workTaskRepository.ListAsync());
        }

        [Fact]
        public async Task AddAsync_WorkTaskAlreadyExist_NotAdded()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            string tasksGroupName = "group1";

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create(tasksGroupName, A.Fake<ILogger>());
            IWorkTask workTask = tasksGroup.CreateTask("taskDescription");

            database.Entities.Add(tasksGroup);

            Assert.Single(await workTaskRepository.ListAsync());

            await workTaskRepository.AddAsync(workTask);

            Assert.NotNull(await workTaskRepository.FindAsync(workTask.ID));
            Assert.Single(await workTaskRepository.ListAsync());
        }

        [Fact]
        public async Task AddAsync_ParentGroupNotExist_NotAdded()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
            
            database.Entities.Add(tasksGroup);

            WorkTask workTask = new WorkTask("group2", "worktask1", A.Fake<ILogger>());

            Assert.Empty(await workTaskRepository.ListAsync());

            await workTaskRepository.AddAsync(workTask);

            Assert.Null(await workTaskRepository.FindAsync(workTask.ID));
            Assert.Empty(await workTaskRepository.ListAsync());
        }

        [Fact]
        public async Task AddAsync_DescriptionExistsInTheSameGroup_NotAdded()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            string tasksGroupName = "group1";
            string taskDescription = "workTask1";

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create(tasksGroupName, A.Fake<ILogger>());
            tasksGroup.CreateTask(taskDescription);

            database.Entities.Add(tasksGroup);

            WorkTask workTask = new WorkTask(tasksGroupName, taskDescription, A.Fake<ILogger>());

            Assert.Single(await workTaskRepository.ListAsync());

            await workTaskRepository.AddAsync(workTask);

            Assert.NotNull(await workTaskRepository.FindAsync(workTask.ID));
            Assert.Single(await workTaskRepository.ListAsync());
        }

        [Fact]
        public async Task AddAsync_DatabaseIsLoadedAndSavedOnce()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            await workTaskRepository.AddAsync(A.Fake<IWorkTask>());

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task FindAsync_IdExist_Found()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
            IWorkTask workTask = tasksGroup.CreateTask("taskDescription");

            database.Entities.Add(tasksGroup);

            Assert.NotNull(await workTaskRepository.FindAsync(workTask.ID));
        }

        [Fact]
        public async Task FindAsync_IdNotExist_NotFound()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            Assert.Null(await workTaskRepository.FindAsync("1005"));
        }

        [Fact]
        public async Task FindAsync_DatabaseIsLoadedOnceAndNoteSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            await workTaskRepository.FindAsync("group1");

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }

        [Fact]
        public async Task ListAsync_AsExpected()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup1 = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
            IWorkTask workTask1 = tasksGroup1.CreateTask("task1");
            IWorkTask workTask2 = tasksGroup1.CreateTask("task2");

            ITasksGroup tasksGroup2 = mTasksGroupBuilder.Create("group2", A.Fake<ILogger>());
            IWorkTask workTask3 = tasksGroup2.CreateTask("task3");

            database.Entities.Add(tasksGroup1);
            database.Entities.Add(tasksGroup2);

            List<IWorkTask> workTasks = (await workTaskRepository.ListAsync()).ToList();
            Assert.Equal(workTask1, workTasks[0]);
            Assert.Equal(workTask2, workTasks[1]);
            Assert.Equal(workTask3, workTasks[2]);
        }

        [Fact]
        public async Task ListAsync_HasNoTasks_ReturnsEmptyList()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            Assert.Empty(await workTaskRepository.ListAsync());
        }

        [Fact]
        public async Task ListAsync_DatabaseChangedAfterInitialize_ReturnCorrectList()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                DatabaseDirectoryPath = TestFilesDirectory
            });

            AppDbContext database = new AppDbContext(databaseOptions, new JsonSerializerWrapper(), A.Fake<ILogger>());

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            Assert.Equal(17, (await workTaskRepository.ListAsync()).Count());

            File.Copy(mAlternateDatabasePath, database.DatabaseFilePath);

            Assert.Equal(15, (await workTaskRepository.ListAsync()).Count());
        }

        [Fact]
        public async Task ListAsync_DatabaseIsLoadedOnceAndNoteSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            await workTaskRepository.ListAsync();

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }

        [Fact]
        public async Task RemoveAsync_TaskExists_TaskRemoved()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup1 = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
            IWorkTask workTask = tasksGroup1.CreateTask("taskDescription");

            database.Entities.Add(tasksGroup1);

            Assert.Single(await workTaskRepository.ListAsync());

            await workTaskRepository.RemoveAsync(workTask);

            Assert.Empty(await workTaskRepository.ListAsync());
        }

        [Fact]
        public async Task RemoveAsync_WorkTaskNotExists_RemoveNotPerformed()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            string taskDescription = "taskDescripition";

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
            tasksGroup.CreateTask(taskDescription);

            database.Entities.Add(tasksGroup);

            Assert.Single(await workTaskRepository.ListAsync());

            WorkTask differentWorkTaskWithSameGroupNameAndDescription = new WorkTask("group1", taskDescription, A.Fake<ILogger>());

            await workTaskRepository.RemoveAsync(differentWorkTaskWithSameGroupNameAndDescription);

            Assert.Single(await workTaskRepository.ListAsync());
        }

        [Fact]
        public async Task RemoveAsync_DatabaseIsLoadedAndSavedOnce()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            await workTaskRepository.RemoveAsync(A.Fake<IWorkTask>());

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateAsync_WorkTaskExists_WorkTaskUpdated()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
            IWorkTask workTask = tasksGroup.CreateTask("taskDescription");

            database.Entities.Add(tasksGroup);

            Assert.Single(await workTaskRepository.ListAsync());

            IWorkTask workTaskWithUpdate = await workTaskRepository.FindAsync(workTask.ID);

            string newTaskSecription = "description_changed";

            workTaskWithUpdate.Description = newTaskSecription;

            await workTaskRepository.UpdateAsync(workTaskWithUpdate);

            Assert.Single(await workTaskRepository.ListAsync());

            IWorkTask updatedTasksGroup = await workTaskRepository.FindAsync(workTaskWithUpdate.ID);

            Assert.Equal(newTaskSecription, updatedTasksGroup.Description);
        }

        [Fact]
        public async Task UpdateAsync_WorkTaskNotExists_WorkTaskNotAdded()
        {
            AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            WorkTaskRepository WorkTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
            tasksGroup.CreateTask("description1");

            database.Entities.Add(tasksGroup);

            Assert.Single(await WorkTaskRepository.ListAsync());

            WorkTask workTask = new WorkTask("group1", "description2", A.Fake<ILogger>());

            await WorkTaskRepository.UpdateAsync(workTask);

            Assert.Single(await WorkTaskRepository.ListAsync());
        }

        [Fact]
        public async Task UpdateAsync_DatabaseIsLoadedAndSavedOnce()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository WorkTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            await WorkTaskRepository.UpdateAsync(A.Fake<IWorkTask>());

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustHaveHappenedOnceExactly();
        }
    }
}