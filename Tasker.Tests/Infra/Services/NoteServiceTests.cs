using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using ObjectSerializer.JsonService;
using System.IO;
using System.Threading.Tasks;
using Takser.Infra.Options;
using TaskData;
using TaskData.Notes;
using Tasker.App.Resources.Note;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Tasker.Domain.Models;
using Tasker.Infra.Consts;
using Tasker.Infra.Services;
using Xunit;

namespace Tasker.Tests.Infra.Services
{
    public class NoteServiceTests
    {
        private const string TestFilesDirectory = "TestFiles";
        private const string GeneralNotesDirectoryName = "GeneralNotes";
        private const string PrivateNotesDirectoryName = "TaskNotes";
        private readonly string GeneralNotesDirectoryPath = Path.Combine(TestFilesDirectory, GeneralNotesDirectoryName);
        private readonly string PrivateNotesDirectoryPath = Path.Combine(TestFilesDirectory, PrivateNotesDirectoryName);

        private readonly INoteFactory mNoteFactory;

        public NoteServiceTests()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.UseTaskerDataEntities();
            serviceCollection.UseJsonObjectSerializer();
            ServiceProvider serviceProvider = serviceCollection
                .AddLogging()
                .BuildServiceProvider();

            mNoteFactory = serviceProvider.GetRequiredService<INoteFactory>();
        }

        [Fact]
        public async Task GetGeneralNotesPaths_AsExpected()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = GeneralNotesDirectoryPath,
                NotesTasksDirectoryPath = "a"
            });

            INoteService noteService = new NoteService(
                mNoteFactory, databaseOptions, NullLogger<NoteService>.Instance);
            NoteNode noteNode = await noteService.GetGeneralNotesStructure().ConfigureAwait(false);

            Assert.Equal(Path.GetFileName(GeneralNotesDirectoryPath), noteNode.Name);
        }

        [Theory]
        [InlineData("generalNote3.txt", "generalNote3.txt", "gn3")]
        [InlineData("generalNote3", "generalNote3.txt", "gn3")]
        [InlineData(@"subject1\generalNote2.txt", @"subject1\generalNote2.txt", "This is generel note 2")]
        [InlineData(@"subject1\generalNote2", @"subject1\generalNote2.txt", "This is generel note 2")]
        public async Task GetNote_AsExpected(string noteRelativePath, string expectedRelativePath, string expectedText)
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = GeneralNotesDirectoryPath,
                NotesTasksDirectoryPath = "a",
            });

            string expectedNotePath = Path.Combine(GeneralNotesDirectoryPath, expectedRelativePath);

            INoteService noteService = new NoteService(
                mNoteFactory, databaseOptions, NullLogger<NoteService>.Instance);
            IResponse<NoteResourceResponse> response = await noteService.GetGeneralNote(noteRelativePath).ConfigureAwait(false);

            Assert.True(response.IsSuccess);
            Assert.Equal(expectedNotePath, response.ResponseObject.Note.NotePath);
            Assert.Equal(expectedText, response.ResponseObject.Note.Text);
        }

        [Fact]
        public async Task GetNote_NoteNotExist_NotNotFound()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = GeneralNotesDirectoryPath,
                NotesTasksDirectoryPath = "a",
            });

            string noteRelativePath = Path.Combine(GeneralNotesDirectoryPath, "not_real_note.txt");

            INoteService noteService = new NoteService(
                mNoteFactory, databaseOptions, NullLogger<NoteService>.Instance);
            IResponse<NoteResourceResponse> response = await noteService.GetGeneralNote(noteRelativePath).ConfigureAwait(false);

            Assert.False(response.IsSuccess);
        }

        [Theory]
        [InlineData("good_relative_note_path", "good_relative_note_path.txt", "some_text")]
        [InlineData("good_relative_note_path.txt", "good_relative_note_path.txt", "some_text")]
        public async Task CreatePrivateNote_RelativePath_NewNotSaved(string noteRelativePath, string expectedRelativePath, string noteText)
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = GeneralNotesDirectoryPath,
                NotesTasksDirectoryPath = PrivateNotesDirectoryPath
            });

            string expectedNotePath = Path.Combine(PrivateNotesDirectoryPath, expectedRelativePath);

            INoteService noteService = new NoteService(
                mNoteFactory, databaseOptions, NullLogger<NoteService>.Instance);

            try
            {
                IResponse<NoteResourceResponse> response =
                    await noteService.CreatePrivateNote(noteRelativePath, noteText).ConfigureAwait(false);

                Assert.True(response.IsSuccess);
                Assert.Equal(expectedNotePath, response.ResponseObject.Note.NotePath);
                Assert.Equal(noteText, response.ResponseObject.Note.Text);
            }
            finally
            {
                if (File.Exists(expectedNotePath))
                    File.Delete(expectedNotePath);
            }
        }

        [Theory]
        [InlineData(TestFilesDirectory, PrivateNotesDirectoryName, "noteName.txt", "some_text")]
        [InlineData(TestFilesDirectory, PrivateNotesDirectoryName, "noteName", "some_text")]
        public async Task CreatePrivateNote_FullPath_NoteCreated(string pathPart1, string pathPart2, string noteName,
            string noteText)
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = GeneralNotesDirectoryPath,
                NotesTasksDirectoryPath = PrivateNotesDirectoryPath
            });

            string notePath = Path.Combine(pathPart1, pathPart2, noteName);
            notePath = Path.ChangeExtension(notePath, AppConsts.NoteExtension);

            INoteService noteService = new NoteService(
                mNoteFactory, databaseOptions, NullLogger<NoteService>.Instance);

            try
            {
                IResponse<NoteResourceResponse> response =
                    await noteService.CreatePrivateNote(notePath, noteText).ConfigureAwait(false);

                Assert.True(response.IsSuccess);
                Assert.Equal(notePath, response.ResponseObject.Note.NotePath);
                Assert.Equal(noteText, response.ResponseObject.Note.Text);
            }
            finally
            {
                if (File.Exists(notePath))
                    File.Delete(notePath);
            }
        }

        [Fact]
        public async Task CreatePrivateNote_PathNoteWithDirectories_NoteSavedWithoutDirectory()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = GeneralNotesDirectoryPath,
                NotesTasksDirectoryPath = PrivateNotesDirectoryPath
            });

            const string noteName = "noteName1";
            const string noteText = "some_text";
            string notePath = Path.Combine(TestFilesDirectory, "not_private_notes_directory", noteName);
            string expectedNotePath = Path.Combine(TestFilesDirectory, PrivateNotesDirectoryName, noteName);
            expectedNotePath = Path.ChangeExtension(expectedNotePath, AppConsts.NoteExtension);

            INoteService noteService = new NoteService(
                mNoteFactory, databaseOptions, NullLogger<NoteService>.Instance);

            try
            {
                IResponse<NoteResourceResponse> response =
                    await noteService.CreatePrivateNote(notePath, noteText).ConfigureAwait(false);

                Assert.True(response.IsSuccess);
                Assert.Equal(expectedNotePath, response.ResponseObject.Note.NotePath);
                Assert.Equal(noteText, response.ResponseObject.Note.Text);
            }
            finally
            {
                if (File.Exists(expectedNotePath))
                    File.Delete(expectedNotePath);
            }
        }
    }
}