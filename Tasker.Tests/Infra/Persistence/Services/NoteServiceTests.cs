using FakeItEasy;
using Logger.Contracts;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using Takser.Infra.Options;
using TaskData;
using TaskData.Contracts;
using Tasker.App.Services;
using Tasker.Domain.Models;
using Tasker.Infra.Services;
using Xunit;

namespace Tasker.Tests.Infra.Persistence.Services
{
    public class NoteServiceTests
    {
        private const string TestFilesDirectory = "TestFiles";
        private readonly string GeneralNotesDirectoryPath = Path.Combine(TestFilesDirectory, "GeneralNotes");

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
            string expectedNotePath = Path.Combine(GeneralNotesDirectoryPath, "generalNote3.txt");

            INoteService noteService = new NoteService(new NoteBuilder(), A.Fake<IOptions<DatabaseConfigurtaion>>(), A.Fake<ILogger>());
            INote note = await noteService.GetNote(expectedNotePath);

            Assert.Equal(expectedNotePath, note.NotePath);
            Assert.Equal("gn3", note.Text);
        }

        [Fact]
        public async Task GetNote_RealPathWithoutExtension_NoteFound()
        {
            string expectedNotePath = Path.Combine(GeneralNotesDirectoryPath, "generalNote3");

            INoteService noteService = new NoteService(new NoteBuilder(), A.Fake<IOptions<DatabaseConfigurtaion>>(), A.Fake<ILogger>());
            INote note = await noteService.GetNote(expectedNotePath);

            Assert.Equal(expectedNotePath, note.NotePath);
            Assert.Equal("gn3", note.Text);
        }

        [Fact]
        public async Task GetNote_NoteNotExists_NoteNotFound()
        {
            string expectedNotePath = Path.Combine(GeneralNotesDirectoryPath, "not_real_note_path");

            INoteService noteService = new NoteService(new NoteBuilder(), A.Fake<IOptions<DatabaseConfigurtaion>>(), A.Fake<ILogger>());
            INote note = await noteService.GetNote(expectedNotePath);

            Assert.Null(note);
        }
    }
}