using Database.JsonService;
using FakeItEasy;
using Logger.Contracts;
using Microsoft.Extensions.Options;
using ObjectSerializer.Contracts;
using System;
using System.IO;
using Takser.Infra.Options;
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
        public void Ctor_SerializationSuccess()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                DatabaseDirectoryPath = TestFilesDirectory
            });

            AppDbContext database = new AppDbContext(databaseOptions, new JsonSerializerWrapper(), A.Fake<ILogger>());
            Assert.Equal(2, database.Entities.Count);
            Assert.Equal(3, database.Entities[0].Size);
            Assert.Equal(15, database.Entities[1].Size);
        }
    }
}