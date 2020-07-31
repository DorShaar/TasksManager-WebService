using Database.JsonService;
using FakeItEasy;
using Logger.Contracts;
using Microsoft.Extensions.Options;
using ObjectSerializer.Contracts;
using System;
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
        private readonly string mAlternateDatabasePath = Path.Combine("TestFiles", "tasks_other.db.txt");
        private readonly string mNewDatabaseDirectoryPath = Path.Combine("TestFiles", "NewDatabase");
        private readonly ITasksGroupBuilder mTasksGroupBuilder = new TaskGroupBuilder();

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

                AppDbContext database = new AppDbContext(Options.Create(databaseConfigurtaion), new JsonSerializerWrapper(), A.Fake<ILogger>());

                WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

                ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());

                database.Entities.Add(tasksGroup);

                IWorkTask workTask = tasksGroup.CreateTask("worktask1");

                await workTaskRepository.AddAsync(workTask);

                Assert.NotNull(await workTaskRepository.FindAsync(workTask.ID));
                Assert.Single(await workTaskRepository.ListAsync());
            }
            finally
            {
                Directory.Delete(tempDirectory, recursive: true);
            }
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
        public async Task AddAsync_DatabaseNotLoadedButSavedOnce()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            await workTaskRepository.AddAsync(A.Fake<IWorkTask>());

            A.CallTo(() => database.LoadDatabase()).MustNotHaveHappened();
            A.CallTo(() => database.SaveCurrentDatabase()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task AddAsync_NullWorkTask_DatabaseNotSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            await workTaskRepository.AddAsync(null);

            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
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
            string tempDirectory = CopyDirectoryToTempDirectory(TestFilesDirectory);

            try
            {
                IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
                {
                    DatabaseDirectoryPath = tempDirectory
                });

                AppDbContext database = new AppDbContext(databaseOptions, new JsonSerializerWrapper(), A.Fake<ILogger>());

                WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

                Assert.Equal(18, (await workTaskRepository.ListAsync()).Count());

                File.Copy(mAlternateDatabasePath, database.DatabaseFilePath, overwrite: true);

                Assert.Equal(16, (await workTaskRepository.ListAsync()).Count());
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

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            await workTaskRepository.ListAsync();

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }

        [Fact]
        public async Task RemoveAsync_NullWorkTask_DatabaseNotSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            await workTaskRepository.RemoveAsync(null);

            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }

        [Fact]
        public async Task UpdateAsync_DatabaseNotLoadedButSavedOnce()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository WorkTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            await WorkTaskRepository.UpdateAsync(A.Fake<IWorkTask>());

            A.CallTo(() => database.LoadDatabase()).MustNotHaveHappened();
            A.CallTo(() => database.SaveCurrentDatabase()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateAsync_NullWorkTask_DatabaseNotSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

            await workTaskRepository.UpdateAsync(null);

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