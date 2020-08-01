using Logger.Contracts;
using Microsoft.Extensions.Options;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Takser.Infra.Options;
using TaskData.Contracts;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Tasker.Domain.Models;

namespace Tasker.Infra.Services
{
    public class NoteService : INoteService
    {
        private const string TextFileExtension = ".txt";

        private readonly string mGeneralNotesDirectory;
        private readonly string mTasksNotesDirectory;
        private readonly INoteBuilder mNoteBuilder;
        private readonly ILogger mLogger;

        public NoteService(INoteBuilder noteBuilder, IOptions<DatabaseConfigurtaion> options, ILogger logger)
        {
            mNoteBuilder = noteBuilder;
            mGeneralNotesDirectory = options.Value.NotesDirectoryPath;
            mTasksNotesDirectory = options.Value.NotesTasksDirectoryPath;
            mLogger = logger;
        }

        public async Task<IResponse<INote>> GetGeneralNote(string noteIdentifier)
        {
            mLogger.Log($"Creating notes file system structure from {mGeneralNotesDirectory}");

            noteIdentifier = AddExtensionIfNotExists(noteIdentifier);

            INote note = mNoteBuilder.Load(Path.Combine(mGeneralNotesDirectory, noteIdentifier));

            if (!File.Exists(note.NotePath))
                return new FailResponse<INote>($"No note found in path {note.NotePath}");

            return new SuccessResponse<INote>(note);
        }

        public async Task<IResponse<INote>> GetTaskNote(string noteIdentifier)
        {
            mLogger.Log($"Creating notes file system structure from {mGeneralNotesDirectory}");

            string taskNotePath = Directory.EnumerateFiles(mTasksNotesDirectory).FirstOrDefault(
                note => Path.GetFileName(note).StartsWith(noteIdentifier));

            if (taskNotePath == null)
                return new FailResponse<INote>($"No note {noteIdentifier} found");

            INote note = mNoteBuilder.Load(taskNotePath);

            return new SuccessResponse<INote>(note);
        }

        private string AddExtensionIfNotExists(string fileName)
        {
            if (!Path.GetExtension(fileName).Equals(TextFileExtension))
                fileName = $"{fileName}{TextFileExtension}";

            return fileName;
        }

        public Task<NoteNode> GetNotesStructure()
        {
            mLogger.Log($"Creating notes file system structure from {mGeneralNotesDirectory}");
            return Task.FromResult(new NoteNode(mGeneralNotesDirectory));
        }
    }
}