using System.Threading.Tasks;
using Tasker.App.Resources;
using Tasker.Domain.Communication;
using Tasker.Domain.Models;

namespace Tasker.App.Services
{
    public interface INoteService
    {
        Task<NoteNode> GetNotesStructure();
        Task<IResponse<NoteResource>> GetGeneralNote(string noteIdentifier);
        Task<IResponse<NoteResource>> GetTaskNote(string noteIdentifier);
    }
}