using System.Threading.Tasks;
using TaskData.Contracts;
using Tasker.Domain.Models;

namespace Tasker.App.Services
{
    public interface INoteService
    {
        Task<NoteNode> GetNotesStructure();
        Task<INote> GetNote(string noteIdentifier);
    }
}