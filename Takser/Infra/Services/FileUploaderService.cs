using Microsoft.Extensions.Hosting;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Takser.Infra.Services
{
    public class FileUploaderService : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await UploadFile("");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task UploadFile(string filePath)
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            byte[] fileContent = await File.ReadAllBytesAsync(filePath);
            multiContent.Add(new ByteArrayContent(fileContent), "files", Path.GetFileName(filePath));

            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.PostAsync($"{baseUrl}/api/FileUpload", multiContent)
                .ConfigureAwait(false);
        }
    }
}