using Database.JsonService;
using FakeItEasy;
using Logger.Contracts;
using Microsoft.Extensions.Options;
using ObjectSerializer.Contracts;
using System;
using System.IO;
using System.Threading.Tasks;
using Takser.Infra.Options;
using TaskData;
using TaskData.Contracts;
using Tasker.Infra.Persistence.Context;
using Xunit;

namespace Tasker.Tests.Infra.Persistence.Context
{
    public class AppDbContextTests
    {
        private const string TestFilesDirectory = "TestFiles";

        [Fact]
        public void DatabaseFilePath_FileExists_AsExpected()
        {
            string tempDirectoryPath = Directory.CreateDirectory(Guid.NewGuid().ToString()).FullName;

            try
            {
                IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
                {
                    DatabaseDirectoryPath = tempDirectoryPath
                });

                AppDbContext database = new AppDbContext(databaseOptions, A.Fake<IObjectSerializer>(), A.Fake<ILogger>());
                Assert.Equal(Path.Combine(tempDirectoryPath, "tasks.db"), database.DatabaseFilePath);
            }
            finally
            {
                Directory.Delete(tempDirectoryPath, recursive: true);
            }
        }

        [Fact]
        public void DatabaseFilePath_FileNotExist_Null()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                DatabaseDirectoryPath = @"\abc"
            });

            AppDbContext database = new AppDbContext(databaseOptions, A.Fake<IObjectSerializer>(), A.Fake<ILogger>());
            Assert.Null(database.DatabaseFilePath);
        }

        [Fact]
        public void DefaultTasksGroup_SameAsInConfiguration()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                DefaultTasksGroup = "abc"
            });

            AppDbContext database = new AppDbContext(databaseOptions, A.Fake<IObjectSerializer>(), A.Fake<ILogger>());
            Assert.Equal("abc", database.DefaultTasksGroup);
        }

        [Fact]
        public void NotesDirectoryPath_SameAsInConfiguration()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = "abc"
            });

            AppDbContext database = new AppDbContext(databaseOptions, A.Fake<IObjectSerializer>(), A.Fake<ILogger>());
            Assert.Equal("abc", database.NotesDirectoryPath);
        }

        [Fact]
        public void NotesTasksDirectoryPath_SameAsInConfiguration()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesTasksDirectoryPath = "abc"
            });

            AppDbContext database = new AppDbContext(databaseOptions, A.Fake<IObjectSerializer>(), A.Fake<ILogger>());
            Assert.Equal("abc", database.NotesTasksDirectoryPath);
        }

        [Fact]
        public async Task LoadDatabase_EntitiesLoadedAsExpected()
        {
            string tempDirectory = CopyDirectoryToTempDirectory();

            try
            {
                IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
                {
                    DatabaseDirectoryPath = tempDirectory
                });

                AppDbContext database = new AppDbContext(databaseOptions, new JsonSerializerWrapper(), A.Fake<ILogger>());
                await database.LoadDatabase().ConfigureAwait(false);

                Assert.Equal(2, database.Entities.Count);
                Assert.Equal(3, database.Entities[0].Size);
                Assert.Equal(15, database.Entities[1].Size);
            }
            finally
            {
                Directory.Delete(tempDirectory, recursive: true);
            }
        }

        [Fact]
        public async Task LoadDatabase_NextIdToProduceLoadedAsExpected()
        {
            string tempDirectory = CopyDirectoryToTempDirectory();

            try
            {
                IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
                {
                    DatabaseDirectoryPath = tempDirectory
                });

                AppDbContext database = new AppDbContext(databaseOptions, new JsonSerializerWrapper(), A.Fake<ILogger>());
                await database.LoadDatabase().ConfigureAwait(false);

                ITasksGroupBuilder tasksGroupBuilder = new TaskGroupBuilder();
                ITasksGroup tasksGroup = tasksGroupBuilder.Create("group", A.Fake<ILogger>());

                Assert.Equal("1022", tasksGroup.ID);
            }
            finally
            {
                Directory.Delete(tempDirectory, recursive: true);
            }
        }

        [Fact]
        public async Task SaveDatabase_EntitiesSavedAsExpected()
        {
            string tempDirectory = Directory.CreateDirectory(Guid.NewGuid().ToString()).FullName;

            try
            {
                IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
                {
                    DatabaseDirectoryPath = tempDirectory
                });

                AppDbContext database = new AppDbContext(databaseOptions, new JsonSerializerWrapper(), A.Fake<ILogger>());

                ITasksGroupBuilder mTasksGroupBuilder = new TaskGroupBuilder();
                ITasksGroup tasksGroup1 = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
                tasksGroup1.CreateTask("workTask1");
                tasksGroup1.CreateTask("workTask2");

                ITasksGroup tasksGroup2 = mTasksGroupBuilder.Create("group2", A.Fake<ILogger>());
                tasksGroup2.CreateTask("workTask3");

                database.Entities.Add(tasksGroup1);
                database.Entities.Add(tasksGroup2);

                await database.SaveCurrentDatabase().ConfigureAwait(false);
                File.Exists(database.DatabaseFilePath);
            }
            finally
            {
                Directory.Delete(tempDirectory, recursive: true);
            }
        }

        private string CopyDirectoryToTempDirectory()
        {
            string tempDirectory = Directory.CreateDirectory(Guid.NewGuid().ToString()).FullName;

            foreach (string filePath in Directory.EnumerateFiles(TestFilesDirectory))
            {
                File.Copy(filePath, Path.Combine(tempDirectory, Path.GetFileNameWithoutExtension(filePath)));
            }

            return tempDirectory;
        }
    }
}