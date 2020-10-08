using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tasker.App.Services;

namespace Tasker.Infra.Services
{
    public class ArchiverService : IArchiverService
    {
        private const string SevenZipExecutableName = "7z";
        private readonly ProcessStartInfo mProcessStartInfo;

        private readonly ILogger<ArchiverService> mLogger;

        public string ArchiveExtension => ".7z";

        public ArchiverService(ILogger<ArchiverService> logger)
        {
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));

            mProcessStartInfo = new ProcessStartInfo
            {
                FileName = SevenZipExecutableName,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
            };
        }

        public async Task<int> Extract(string archivePath, string directoryToExtract, string password)
        {
            mProcessStartInfo.ArgumentList.Clear();
            mProcessStartInfo.ArgumentList.Add("x");
            mProcessStartInfo.ArgumentList.Add(archivePath);
            mProcessStartInfo.ArgumentList.Add($"-o{directoryToExtract}");
            mProcessStartInfo.ArgumentList.Add($"-p{password}");

            return await RunProcess().ConfigureAwait(false);
        }

        public async Task<int> Pack(string directoryToPack, string packedArchivePath, string password)
        {
            mProcessStartInfo.ArgumentList.Clear();
            mProcessStartInfo.ArgumentList.Add("a");
            mProcessStartInfo.ArgumentList.Add(packedArchivePath);
            mProcessStartInfo.ArgumentList.Add(directoryToPack);
            mProcessStartInfo.ArgumentList.Add($"-p{password}");

            return await RunProcess().ConfigureAwait(false);
        }

        private async Task<int> RunProcess()
        {
            Process process = new Process
            {
                StartInfo = mProcessStartInfo
            };

            process.Start();
            process.WaitForExit(10000);

            string standardOutput = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
            string standardError = await process.StandardError.ReadToEndAsync().ConfigureAwait(false);

            mLogger.LogDebug(standardOutput);

            if (!string.IsNullOrWhiteSpace(standardError))
                mLogger.LogError(standardError);

            return process.ExitCode;
        }
    }
}