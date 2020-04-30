using FakeItEasy;
using Logger.Contracts;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using Takser.Infra.Options;
using TaskData.Contracts;
using Tasker.App.Resources;
using Tasker.App.Services;
using Tasker.Infra.Services;
using Xunit;

namespace Tasker.Tests.Infra.Persistence.Services
{
    public class NoteServiceTests
    {
        private const string TestFilesDirectory = "TestFiles";
        private const string GeneralNotesDirectoryName = "GeneralNotes";

        [Fact]
        public async Task GetAllNotesPaths_AsExpected()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = Path.Combine(TestFilesDirectory, GeneralNotesDirectoryName)
            });
            
            INoteService noteService = new NoteService(databaseOptions, A.Fake<ILogger>());
            NoteNode noteNode = await noteService.GetNotesStructure();

            Assert.Equal(GeneralNotesDirectoryName, noteNode.Name);
        }

        [Fact]
        public async Task GetNote_RealPathWithExtension_NoteFound()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = Path.Combine(TestFilesDirectory, GeneralNotesDirectoryName)
            });

            string expectedNotePath = Path.Combine(GeneralNotesDirectoryName, "generalNote3.txt");

            INoteService noteService = new NoteService(databaseOptions, A.Fake<ILogger>());
            INote note = await noteService.GetNote(expectedNotePath);

            Assert.Equal(expectedNotePath, note.NotePath);
            Assert.Equal("gn3", note.Text);
        }

        [Fact]
        public async Task GetNote_RealPathWithoutExtension_NoteFound()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = Path.Combine(TestFilesDirectory, GeneralNotesDirectoryName)
            });

            string expectedNotePath = Path.Combine(GeneralNotesDirectoryName, "generalNote3");

            INoteService noteService = new NoteService(databaseOptions, A.Fake<ILogger>());
            INote note = await noteService.GetNote(expectedNotePath);

            Assert.Equal(expectedNotePath, note.NotePath);
            Assert.Equal("gn3", note.Text);
        }

        [Fact]
        public async Task GetNote_NoteNotExists_NoteNotFound()
        {
            IOptions<DatabaseConfigurtaion> databaseOptions = Options.Create(new DatabaseConfigurtaion()
            {
                NotesDirectoryPath = Path.Combine(TestFilesDirectory, GeneralNotesDirectoryName)
            });

            string expectedNotePath = Path.Combine(GeneralNotesDirectoryName, "not_real_note_path");

            INoteService noteService = new NoteService(databaseOptions, A.Fake<ILogger>());
            INote note = await noteService.GetNote(expectedNotePath);

            Assert.Null(note);
        }
    }
}