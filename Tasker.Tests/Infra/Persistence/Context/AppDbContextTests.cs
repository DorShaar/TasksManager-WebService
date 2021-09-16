using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using Takser.Infra.Options;
using TaskData.IDsProducer;
using TaskData.Ioc;
using TaskData.ObjectSerializer.JsonService;
using TaskData.TasksGroups;
using TaskData.TasksGroups.Producers;
using TaskData.WorkTasks.Producers;
using Tasker.Infra.Consts;
using Tasker.Infra.Persistence.Context;
using Xunit;

namespace Tasker.Tests.Infra.Persistence.Context
{
    public class AppDbContextTests
    {
        private const string TestFilesDirectory = "TestFiles";

        private readonly ITasksGroupFactory mTasksGroupFactory;
        private readonly IWorkTaskProducer mWorkTaskProducer;
        private readonly ITasksGroupProducer mTasksGroupProducer;
        private readonly IObjectSerializer mObjectSerializer;
        private readonly IIDProducer mIDProducer;

        public AppDbContextTests()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.UseTaskerDataEntities()
                .UseJsonObjectSerializer()
                .RegisterRegularWorkTaskProducer()
                .RegisterRegularTasksGroupProducer();

            ServiceProvider serviceProvider = serviceCollection
                .AddLogging()
                .BuildServiceProvider();

            mTasksGroupFactory = serviceProvider.GetRequiredService<ITasksGroupFactory>();
            mWorkTaskProducer = serviceProvider.GetRequiredService<IWorkTaskProducer>();
            mTasksGroupProducer = serviceProvider.GetRequiredService<ITasksGroupProducer>();
            mObjectSerializer = serviceProvider.GetRequiredService<IObjectSerializer>();
            mIDProducer = serviceProvider.GetRequiredService<IIDProducer>();
        }

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

                AppDbContext database = new AppDbContext(
                    databaseOptions,
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

                Assert.Equal(Path.Combine(tempDirectoryPath, AppConsts.DatabaseName), database.DatabaseFilePath);
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

            AppDbContext database = new AppDbContext(
                   databaseOptions,
                   mObjectSerializer,
                   mIDProducer,
                   NullLogger<AppDbContext>.Instance);

            Assert.Null(database.DatabaseFilePath);
        }

        [Fact]
        public void NotesDirectoryPath_SameAsInConfiguration()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = "abc"
            });

            AppDbContext database = new AppDbContext(
                   databaseOptions,
                   mObjectSerializer,
                   mIDProducer,
                   NullLogger<AppDbContext>.Instance);

            Assert.Equal("abc", database.NotesDirectoryPath);
        }

        [Fact]
        public void NotesTasksDirectoryPath_SameAsInConfiguration()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesTasksDirectoryPath = "abc"
            });

            AppDbContext database = new AppDbContext(
                   databaseOptions,
                   mObjectSerializer,
                   mIDProducer,
                   NullLogger<AppDbContext>.Instance);

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

                AppDbContext database = new AppDbContext(
                   databaseOptions,
                   mObjectSerializer,
                   mIDProducer,
                   NullLogger<AppDbContext>.Instance);

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

                AppDbContext database = new AppDbContext(
                   databaseOptions,
                   mObjectSerializer,
                   mIDProducer,
                   NullLogger<AppDbContext>.Instance);

                await database.LoadDatabase().ConfigureAwait(false);

                ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("group", mTasksGroupProducer).Value;

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

                AppDbContext database = new AppDbContext(
                   databaseOptions,
                   mObjectSerializer,
                   mIDProducer,
                   NullLogger<AppDbContext>.Instance);

                ITasksGroup tasksGroup1 = mTasksGroupFactory.CreateGroup("group1", mTasksGroupProducer).Value;
                mTasksGroupFactory.CreateTask(tasksGroup1, "workTask1", mWorkTaskProducer);
                mTasksGroupFactory.CreateTask(tasksGroup1, "workTask2", mWorkTaskProducer);

                ITasksGroup tasksGroup2 = mTasksGroupFactory.CreateGroup("group2", mTasksGroupProducer).Value;
                mTasksGroupFactory.CreateTask(tasksGroup2, "workTask3", mWorkTaskProducer);

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

        private static string CopyDirectoryToTempDirectory()
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