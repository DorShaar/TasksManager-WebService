using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using Takser.Infra.Options;
using Tasker.Infra.Services;
using Xunit;

namespace Tasker.Tests.Infra.Services
{
    public class GoogleDriveUploadServiceTests
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

            GoogleDriveCloudService googleDriveUploadService =
                new GoogleDriveCloudService(databaseOptions, NullLogger<GoogleDriveCloudService>.Instance);

            Assert.True(await googleDriveUploadService.Upload("Tasker-web-test").ConfigureAwait(false));
        }
    }
}