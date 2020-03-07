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
            await UploadFile(@"C:\Users\Dor Shaar\OneDrive\Desktop\TestFile.txt");
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

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.PostAsync(
                $"https://localhost:44337/api/FileUpload", multiContent)
                .ConfigureAwait(false);

                using (response)
                {
                    response.EnsureSuccessStatusCode();
                }
            }
        }
    }
}