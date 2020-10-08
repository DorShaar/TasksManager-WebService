using Microsoft.Extensions.Logging.Abstractions;
using System.IO;
using System.Threading.Tasks;
using Tasker.Infra.Services;
using Xunit;

namespace Tasker.Tests.Infra.Services
{
    public class ArchiverServiceTests
    {
        private const string TestFilesDirectory = "TestFiles";
        private readonly string ArchiverTestFilesDirectory = Path.Combine(TestFilesDirectory, "Archiver");

        [Fact]
        public void ArchiveExtension_Is7z()
        {
            ArchiverService archiverService = new ArchiverService(NullLogger<ArchiverService>.Instance);

            Assert.Equal(".7z", archiverService.ArchiveExtension);
        }

        [Fact]
        public async Task Extract_ExitCode_0()
        {
            ArchiverService archiverService = new ArchiverService(NullLogger<ArchiverService>.Instance);

            string archiveToExtract = Path.Combine(ArchiverTestFilesDirectory, "AfterPack", "Archiver.7z");

            string tempOutputDirectory = Path.GetRandomFileName();

            try
            {
                int exitCode =
                    await archiverService.Extract(archiveToExtract, tempOutputDirectory, "1234").ConfigureAwait(false);

                Assert.Equal(0, exitCode);
                Assert.Equal(
                    4,
                    Directory.GetFileSystemEntries(tempOutputDirectory, "*", SearchOption.AllDirectories).Length);
            }
            finally
            {
                if (Directory.Exists(tempOutputDirectory))
                    Directory.Delete(tempOutputDirectory, recursive: true);
            }
        }

        [Fact]
        public async Task Pack_ExitCode_0()
        {
            ArchiverService archiverService = new ArchiverService(NullLogger<ArchiverService>.Instance);

            string directoryToPack = Path.Combine(ArchiverTestFilesDirectory, "BeforePack");

            string packedArchivePath = Path.GetRandomFileName();

            try
            {
                int exitCode =
                    await archiverService.Pack(directoryToPack, packedArchivePath, "1234").ConfigureAwait(false);

                Assert.Equal(0, exitCode);
            }
            finally
            {
                if (File.Exists(packedArchivePath))
                    File.Delete(packedArchivePath);
            }
        }
    }
}