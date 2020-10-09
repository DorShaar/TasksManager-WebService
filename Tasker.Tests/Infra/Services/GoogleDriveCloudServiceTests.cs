using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using Takser.Infra.Options;
using Tasker.App.Services;
using Tasker.Infra.Options;
using Tasker.Infra.Services;
using Xunit;

namespace Tasker.Tests.Infra.Services
{
    public class GoogleDriveCloudServiceTests
    {
        private const string TestFilesDirectory = "TestFiles";
        private const string TaskNotesDirectoryName = "TaskNotes";
        private readonly string TasksNotesDirectoryPath = Path.Combine(TestFilesDirectory, TaskNotesDirectoryName);

        [Fact]
        public async Task Upload_DatabaseUploadedIntoDrive()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                DatabaseDirectoryPath = TestFilesDirectory,
                NotesDirectoryPath = "a",
                NotesTasksDirectoryPath = TasksNotesDirectoryPath
            });

            GoogleDriveCloudService googleDriveCloudService = new GoogleDriveCloudService(
                A.Fake<IArchiverService>(),
                databaseOptions,
                Options.Create(new TaskerConfiguration()),
                NullLogger<GoogleDriveCloudService>.Instance);

            Assert.True(await googleDriveCloudService.Upload("Tasker-web-test").ConfigureAwait(false));
        }

        [Fact]
        public async Task Download_FileExist_StreamWasDownloadedSuccessfully()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                DatabaseDirectoryPath = TestFilesDirectory,
                NotesDirectoryPath = "a",
                NotesTasksDirectoryPath = TasksNotesDirectoryPath
            });

            GoogleDriveCloudService googleDriveCloudService = new GoogleDriveCloudService(
                A.Fake<IArchiverService>(),
                databaseOptions,
                Options.Create(new TaskerConfiguration()),
                NullLogger<GoogleDriveCloudService>.Instance);

            Stream stream = await googleDriveCloudService.Download("9-20-2020.zip").ConfigureAwait(false);

            Assert.NotEqual(Stream.Null, stream);
        }

        [Fact]
        public async Task Download_FileNotExist_EmptyStreamIsGiven()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                DatabaseDirectoryPath = TestFilesDirectory,
                NotesDirectoryPath = "a",
                NotesTasksDirectoryPath = TasksNotesDirectoryPath
            });

            GoogleDriveCloudService googleDriveCloudService = new GoogleDriveCloudService(
                A.Fake<IArchiverService>(),
                databaseOptions,
                Options.Create(new TaskerConfiguration()),
                NullLogger<GoogleDriveCloudService>.Instance);

            Stream stream = await googleDriveCloudService.Download("fdghgj").ConfigureAwait(false);

            Assert.Equal(Stream.Null, stream);
        }
    }
}