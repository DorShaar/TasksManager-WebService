using FakeItEasy;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Options;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Takser.Infra.Options;
using Tasker.App.Services;
using Xunit;

namespace Tasker.Tests.Api.Controllers
{
    public class NotesControllerTests
    {
        private const string MainRoute = "api/Notes";

        [Fact]
        public async Task GetGeneralNotesStructureAsync_SuccessStatusCode()
        {
            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakeDatabaseConfig();
            using HttpClient httpClient = testServer.CreateClient();

            HttpResponseMessage response = await httpClient.GetAsync(MainRoute);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetGeneralNoteAsync_NonValidNotePath_NotFoundCode()
        {
            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakeDatabaseConfig();
            using HttpClient httpClient = testServer.CreateClient();

            HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}/blabla");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("subject1-generalNote2.txt", "This is generel note 2")]
        public async Task GetGeneralNoteAsync_ValidNotePath_CorrectNoteTextIsGiven(string notePath, string expectedText)
        {
            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakeDatabaseConfig();
            using HttpClient httpClient = testServer.CreateClient();

            HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}/{notePath}");

            response.EnsureSuccessStatusCode();
            Assert.Equal(expectedText, await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        }
    }
}