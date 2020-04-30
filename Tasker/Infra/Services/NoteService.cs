using Logger.Contracts;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using Takser.Infra.Options;
using TaskData.Contracts;
using Tasker.App.Services;
using Tasker.Domain.Models;

namespace Tasker.Infra.Services
{
    public class NoteService : INoteService
    {
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

        public Task<INote> GetNote(string noteIdentifier)
        {
            mLogger.Log($"Creating notes file system structure from {mGeneralNotesDirectory}");
            INote note = mNoteBuilder.Load(noteIdentifier);

            if (!File.Exists(note.NotePath))
            {
                mLogger.Log($"No note found in path {note.NotePath}");
                note = null;
            }

            return Task.FromResult(note);
        }

        public Task<NoteNode> GetNotesStructure()
        {
            mLogger.Log($"Creating notes file system structure from {mGeneralNotesDirectory}");
            return Task.FromResult(new NoteNode(mGeneralNotesDirectory));
        }
    }
}