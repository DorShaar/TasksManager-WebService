using System.Threading.Tasks;
using TaskData.Contracts;
using Tasker.App.Resources;

namespace Tasker.App.Services
{
    public interface INoteService
    {
        Task<NoteNode> GetNotesStructure();
        Task<INote> GetNote(string noteIdentifier);
    }
}