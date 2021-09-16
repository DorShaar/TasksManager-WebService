using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tasker.App.Resources.Note;
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

            using HttpResponseMessage response = await httpClient.GetAsync(MainRoute).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetGeneralNoteAsync_NonValidNotePath_NoteFoundCode()
        {
            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakeDatabaseConfig();
            using HttpClient httpClient = testServer.CreateClient();

            using HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}/blabla").ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory(Skip = "no note support yet")]
        [InlineData("subject1*generalNote2.txt", "This is generel note 2")]
        public async Task GetGeneralNoteAsync_ValidNotePath_CorrectNoteTextIsGiven(string notePath, string expectedText)
        {
            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakeDatabaseConfig();
            using HttpClient httpClient = testServer.CreateClient();

            using HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}/{notePath}").ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            NoteResource noteResource = JsonConvert.DeserializeObject<NoteResource>(
                await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            Assert.Equal(expectedText, noteResource.Text);
        }

        [Fact]
        public async Task GetPrivateNoteAsync_NonValidNoteIdentifier_NoteFoundCode()
        {
            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakeDatabaseConfig();
            using HttpClient httpClient = testServer.CreateClient();

            const string notExistingPrivateNote = "1050";
            using HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}/note/{notExistingPrivateNote}").ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory(Skip = "no note support yet")]
        [InlineData("1003", "task number 1003")]
        [InlineData("1003.txt", "task number 1003")]
        [InlineData("1022", "should clean old directories")]
        [InlineData("1022 - clean old directories", "should clean old directories")]
        [InlineData("1022 - clean old directories.txt", "should clean old directories")]
        public async Task GetPrivateNoteAsync_ValidNoteIdentifier_CorrectNoteTextIsGiven(string noteIdentifier, string expectedText)
        {
            using TestServer testServer = ApiTestHelper.CreateTestServerWithFakeDatabaseConfig();
            using HttpClient httpClient = testServer.CreateClient();

            using HttpResponseMessage response = await httpClient.GetAsync($"{MainRoute}/note/{noteIdentifier}").ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            NoteResource noteResource = JsonConvert.DeserializeObject<NoteResource>(
                await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            Assert.Equal(expectedText, noteResource.Text);
        }
    }
}