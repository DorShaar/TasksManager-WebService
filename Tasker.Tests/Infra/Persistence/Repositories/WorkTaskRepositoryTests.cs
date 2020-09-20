using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using ObjectSerializer.JsonService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Takser.App.Persistence.Context;
using Takser.Infra.Options;
using TaskData;
using TaskData.IDsProducer;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
using Tasker.Infra.Persistence.Context;
using Tasker.Infra.Persistence.Repositories;
using Xunit;

namespace Tasker.Tests.Infra.Persistence.Repositories
{
    public class WorkTaskRepositoryTests
    {
        private const string TestFilesDirectory = "TestFiles";
        private readonly string mAlternateDatabasePath = Path.Combine("TestFiles", "tasks_other.db.txt");
        private readonly string mNewDatabaseDirectoryPath = Path.Combine("TestFiles", "NewDatabase");
        private readonly ITasksGroupFactory mTasksGroupBuilder;
        private readonly IObjectSerializer mObjectSerializer;
        private readonly IIDProducer mIDProducer;

        public WorkTaskRepositoryTests()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.UseTaskerDataEntities();
            serviceCollection.UseJsonObjectSerializer();
            ServiceProvider serviceProvider = serviceCollection
                .AddLogging()
                .BuildServiceProvider();

            mTasksGroupBuilder = serviceProvider.GetRequiredService<ITasksGroupFactory>();
            mObjectSerializer = serviceProvider.GetRequiredService<IObjectSerializer>();
            mIDProducer = serviceProvider.GetRequiredService<IIDProducer>();
        }

        [Fact]
        public async Task AddAsync_WorkTaskNotExist_Success()
        {
            string tempDirectory = CopyDirectoryToTempDirectory(mNewDatabaseDirectoryPath);

            try
            {
                DatabaseConfigurtaion databaseConfigurtaion = new DatabaseConfigurtaion
                {
                    DatabaseDirectoryPath = tempDirectory
                };

                AppDbContext database = new AppDbContext(
                    Options.Create(databaseConfigurtaion),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

                WorkTaskRepository workTaskRepository =
                    new WorkTaskRepository(database, NullLogger< WorkTaskRepository>.Instance);

                ITasksGroup tasksGroup = mTasksGroupBuilder.CreateGroup("group1");

                database.Entities.Add(tasksGroup);

                IWorkTask workTask = mTasksGroupBuilder.CreateTask(tasksGroup, "worktask1");

                await workTaskRepository.AddAsync(workTask).ConfigureAwait(false);

                Assert.NotNull(await workTaskRepository.FindAsync(workTask.ID).ConfigureAwait(false));
                Assert.Single(await workTaskRepository.ListAsync().ConfigureAwait(false));
            }
            finally
            {
                Directory.Delete(tempDirectory, recursive: true);
            }
        }

        [Fact]
        public async Task AddAsync_WorkTaskAlreadyExist_NotAdded()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            WorkTaskRepository workTaskRepository =
                new WorkTaskRepository(database, NullLogger<WorkTaskRepository>.Instance);

            const string tasksGroupName = "group1";

            ITasksGroup tasksGroup = mTasksGroupBuilder.CreateGroup(tasksGroupName);
            IWorkTask workTask = mTasksGroupBuilder.CreateTask(tasksGroup, "taskDescription");

            database.Entities.Add(tasksGroup);

            Assert.Single(await workTaskRepository.ListAsync().ConfigureAwait(false));

            await workTaskRepository.AddAsync(workTask).ConfigureAwait(false);

            Assert.NotNull(await workTaskRepository.FindAsync(workTask.ID).ConfigureAwait(false));
            Assert.Single(await workTaskRepository.ListAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task AddAsync_DatabaseNotLoadedButSavedOnce()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository workTaskRepository =
                new WorkTaskRepository(database, NullLogger<WorkTaskRepository>.Instance);

            await workTaskRepository.AddAsync(A.Fake<IWorkTask>()).ConfigureAwait(false);

            A.CallTo(() => database.LoadDatabase()).MustNotHaveHappened();
            A.CallTo(() => database.SaveCurrentDatabase()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task AddAsync_NullWorkTask_DatabaseNotSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository workTaskRepository =
                new WorkTaskRepository(database, NullLogger<WorkTaskRepository>.Instance);

            await workTaskRepository.AddAsync(null).ConfigureAwait(false);

            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }

        [Fact]
        public async Task FindAsync_IdExist_Found()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            WorkTaskRepository workTaskRepository =
                new WorkTaskRepository(database, NullLogger<WorkTaskRepository>.Instance);

            ITasksGroup tasksGroup = mTasksGroupBuilder.CreateGroup("group1");
            IWorkTask workTask = mTasksGroupBuilder.CreateTask(tasksGroup, "taskDescription");

            database.Entities.Add(tasksGroup);

            Assert.NotNull(await workTaskRepository.FindAsync(workTask.ID).ConfigureAwait(false));
        }

        [Fact]
        public async Task FindAsync_IdNotExist_NotFound()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            WorkTaskRepository workTaskRepository =
                new WorkTaskRepository(database, NullLogger<WorkTaskRepository>.Instance);

            Assert.Null(await workTaskRepository.FindAsync("1005").ConfigureAwait(false));
        }

        [Fact]
        public async Task FindAsync_DatabaseIsLoadedOnceAndNoteSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository workTaskRepository =
                new WorkTaskRepository(database, NullLogger<WorkTaskRepository>.Instance);

            await workTaskRepository.FindAsync("group1").ConfigureAwait(false);

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }

        [Fact]
        public async Task ListAsync_AsExpected()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            WorkTaskRepository workTaskRepository =
                new WorkTaskRepository(database, NullLogger<WorkTaskRepository>.Instance);

            ITasksGroup tasksGroup1 = mTasksGroupBuilder.CreateGroup("group1");
            IWorkTask workTask1 = mTasksGroupBuilder.CreateTask(tasksGroup1, "task1");
            IWorkTask workTask2 = mTasksGroupBuilder.CreateTask(tasksGroup1, "task2");

            ITasksGroup tasksGroup2 = mTasksGroupBuilder.CreateGroup("group2");
            IWorkTask workTask3 = mTasksGroupBuilder.CreateTask(tasksGroup2, "task3");

            database.Entities.Add(tasksGroup1);
            database.Entities.Add(tasksGroup2);

            List<IWorkTask> workTasks = (await workTaskRepository.ListAsync().ConfigureAwait(false)).ToList();
            Assert.Equal(workTask1, workTasks[0]);
            Assert.Equal(workTask2, workTasks[1]);
            Assert.Equal(workTask3, workTasks[2]);
        }

        [Fact]
        public async Task ListAsync_HasNoTasks_ReturnsEmptyList()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            WorkTaskRepository workTaskRepository =
                new WorkTaskRepository(database, NullLogger<WorkTaskRepository>.Instance);

            Assert.Empty(await workTaskRepository.ListAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task ListAsync_DatabaseChangedAfterInitialize_ReturnCorrectList()
        {
            string tempDirectory = CopyDirectoryToTempDirectory(TestFilesDirectory);

            try
            {
                IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
                {
                    DatabaseDirectoryPath = tempDirectory
                });

                AppDbContext database = new AppDbContext(
                    databaseOptions,
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

                WorkTaskRepository workTaskRepository =
                new WorkTaskRepository(database, NullLogger<WorkTaskRepository>.Instance);

                Assert.Equal(18, (await workTaskRepository.ListAsync().ConfigureAwait(false)).Count());

                File.Copy(mAlternateDatabasePath, database.DatabaseFilePath, overwrite: true);

                Assert.Equal(16, (await workTaskRepository.ListAsync().ConfigureAwait(false)).Count());
            }
            finally
            {
                Directory.Delete(tempDirectory, recursive: true);
            }
        }

        [Fact]
        public async Task ListAsync_DatabaseIsLoadedOnceAndNoteSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository workTaskRepository =
                new WorkTaskRepository(database, NullLogger<WorkTaskRepository>.Instance);

            await workTaskRepository.ListAsync().ConfigureAwait(false);

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }

        [Fact]
        public async Task RemoveAsync_NullWorkTask_DatabaseNotSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository workTaskRepository =
                new WorkTaskRepository(database, NullLogger<WorkTaskRepository>.Instance);

            await workTaskRepository.RemoveAsync(null).ConfigureAwait(false);

            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateAsync_DatabaseNotLoadedButSavedOnce()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository workTaskRepository =
                new WorkTaskRepository(database, NullLogger<WorkTaskRepository>.Instance);

            await workTaskRepository.UpdateAsync(A.Fake<IWorkTask>()).ConfigureAwait(false);

            A.CallTo(() => database.LoadDatabase()).MustNotHaveHappened();
            A.CallTo(() => database.SaveCurrentDatabase()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateAsync_NullWorkTask_DatabaseNotSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository workTaskRepository =
                new WorkTaskRepository(database, NullLogger<WorkTaskRepository>.Instance);

            await workTaskRepository.UpdateAsync(null).ConfigureAwait(false);

            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }

        private string CopyDirectoryToTempDirectory(string sourceDirectory)
        {
            string tempDirectory = Directory.CreateDirectory(Guid.NewGuid().ToString()).FullName;

            foreach (string filePath in Directory.EnumerateFiles(sourceDirectory))
            {
                File.Copy(filePath, Path.Combine(tempDirectory, Path.GetFileNameWithoutExtension(filePath)));
            }

            return tempDirectory;
        }
    }
}