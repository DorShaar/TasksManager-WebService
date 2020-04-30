using Logger.Contracts;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Takser.Infra.Options;
using TaskData.Contracts;
using Tasker.App.Resources;
using Tasker.App.Services;

namespace Tasker.Infra.Services
{
    public class NoteService : INoteService
    {
        private readonly string mGeneralNotesDirectory;
        private readonly string mTasksNotesDirectory;
        private readonly ILogger mLogger;

        public NoteService(IOptions<DatabaseConfigurtaion> options, ILogger logger)
        {
            mGeneralNotesDirectory = options.Value.NotesDirectoryPath;
            mTasksNotesDirectory = options.Value.NotesTasksDirectoryPath;
            mLogger = logger;
        }

        public Task<INote> GetNote(string noteIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task<NoteNode> GetNotesStructure()
        {
            throw new System.NotImplementedException();
        }
    }
}