using System.Threading.Tasks;
using TaskData.Contracts;
using Tasker.Domain.Communication;
using Tasker.Domain.Models;

namespace Tasker.App.Services
{
    public interface INoteService
    {
        Task<NoteNode> GetNotesStructure();
        Task<IResponse<INote>> GetNote(string noteIdentifier);
    }
}