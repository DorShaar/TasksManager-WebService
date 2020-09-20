using FakeItEasy;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tasker.App.Services;
using Xunit;

namespace Tasker.Tests.Api.Controllers
{
    public class CloudServicesControllerTests
    {
        private const string MainRoute = "api/CloudServices";
        private const string PostMediaType = "application/json";

        [Fact]
        public async Task SaveDatabaseAsync_UploadFails_BadRequestReturned()
        {
            ICloudService cloudService = A.Fake<ICloudService>();
            A.CallTo(() => cloudService.Upload(A<string>.Ignored)).Returns(false);

            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes(cloudService: cloudService);
            using HttpClient httpClient = testServer.CreateClient();

            using StringContent jsonContent = new StringContent("invalidResource", Encoding.UTF8, PostMediaType);
            using HttpResponseMessage response = await httpClient.PutAsync(MainRoute, jsonContent).ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task SaveDatabaseAsync_UploadSuccess_SuccessStatusCodeReturned()
        {
            ICloudService cloudService = A.Fake<ICloudService>();
            A.CallTo(() => cloudService.Upload(A<string>.Ignored)).Returns(true);

            using TestServer testServer = ApiTestHelper.BuildTestServerWithFakes(cloudService: cloudService);
            using HttpClient httpClient = testServer.CreateClient();

            using StringContent jsonContent = new StringContent("invalidResource", Encoding.UTF8, PostMediaType);
            using HttpResponseMessage response = await httpClient.PutAsync(MainRoute, jsonContent).ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}