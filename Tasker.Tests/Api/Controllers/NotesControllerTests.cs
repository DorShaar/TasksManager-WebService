using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Tasker.Tests.Api.Controllers
{
    public class NotesControllerTests
    {
        private const string MainRoute = "api/Notes";

        [Fact]
        public async Task GetGeneralNotesStructureAsync_SuccessStatusCode()
        {
            using TestServer testServer = ApiTestHelper.CreateTestServer();
            using HttpClient httpClient = testServer.CreateClient();

            HttpResponseMessage response = await httpClient.GetAsync(MainRoute);

            response.EnsureSuccessStatusCode();
        }
    }
}