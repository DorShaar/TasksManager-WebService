using System.Threading.Tasks;
using TaskData.Notes;
using Tasker.Domain.Communication;
using Tasker.Domain.Models;

namespace Tasker.App.Services
{
    public interface INoteService
    {
        Task<NoteNode> GetNotesStructure();
        Task<IResponse<INote>> GetGeneralNote(string noteIdentifier);
        Task<IResponse<INote>> GetTaskNote(string noteIdentifier);
    }
}