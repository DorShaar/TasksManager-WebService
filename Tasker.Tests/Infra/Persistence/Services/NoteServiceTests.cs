using FakeItEasy;
using Logger.Contracts;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using Takser.Infra.Options;
using TaskData;
using TaskData.Contracts;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Tasker.Domain.Models;
using Tasker.Infra.Services;
using Xunit;

namespace Tasker.Tests.Infra.Persistence.Services
{
    public class NoteServiceTests
    {
        private const string TestFilesDirectory = "TestFiles";
        private const string GeneralNotesDirectoryName = "GeneralNotes";
        private readonly string GeneralNotesDirectoryPath = Path.Combine(TestFilesDirectory, GeneralNotesDirectoryName);

        [Fact]
        public async Task GetAllNotesPaths_AsExpected()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = GeneralNotesDirectoryPath
            });

            INoteService noteService = new NoteService(A.Fake<INoteBuilder>(), databaseOptions, A.Fake<ILogger>());
            NoteNode noteNode = await noteService.GetNotesStructure();

            Assert.Equal(Path.GetFileName(GeneralNotesDirectoryPath), noteNode.Name);
        }

        [Fact]
        public async Task GetNote_RealPathWithExtension_NoteFound()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = GeneralNotesDirectoryPath
            });

            string noteName = "generalNote3.txt";
            string noteRelativePath = Path.Combine(GeneralNotesDirectoryName, noteName);
            string expectedNotePath = Path.Combine(GeneralNotesDirectoryPath, noteName);

            INoteService noteService = new NoteService(new NoteBuilder(), databaseOptions, A.Fake<ILogger>());
            IResponse<INote> response = await noteService.GetNote(noteRelativePath);

            Assert.True(response.IsSuccess);
            Assert.Equal(expectedNotePath, response.ResponseObject.NotePath);
            Assert.Equal("gn3", response.ResponseObject.Text);
        }

        [Fact]
        public async Task GetNote_RealPathWithoutExtension_NoteFound()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = GeneralNotesDirectoryPath
            });

            string noteName = "generalNote3";
            string noteRelativePath = Path.Combine(GeneralNotesDirectoryName, noteName);
            string expectedNotePath = Path.Combine(GeneralNotesDirectoryPath, noteName) + ".txt";

            INoteService noteService = new NoteService(new NoteBuilder(), databaseOptions, A.Fake<ILogger>());
            IResponse<INote> response = await noteService.GetNote(noteRelativePath);

            Assert.True(response.IsSuccess);
            Assert.Equal(expectedNotePath, response.ResponseObject.NotePath);
            Assert.Equal("gn3", response.ResponseObject.Text);
        }

        [Fact]
        public async Task GetNote_NoteNotExists_NoteNotFound()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = GeneralNotesDirectoryPath
            });

            string noteName = "not_real_note.txt";
            string noteRelativePath = Path.Combine(GeneralNotesDirectoryName, noteName);

            INoteService noteService = new NoteService(new NoteBuilder(), databaseOptions, A.Fake<ILogger>());
            IResponse<INote> response = await noteService.GetNote(noteRelativePath);

            Assert.False(response.IsSuccess);
        }
    }
}