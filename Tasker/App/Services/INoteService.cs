using System.Threading.Tasks;
using Tasker.App.Resources.Note;
using Tasker.Domain.Communication;
using Tasker.Domain.Models;

namespace Tasker.App.Services
{
    public interface INoteService
    {
        Task<NoteNode> GetNotesStructure();
        Task<NoteNode> GetGeneralNotesStructure();
        Task<IResponse<NoteResourceResponse>> GetGeneralNote(string noteIdentifier);
        Task<IResponse<NoteResourceResponse>> GetTaskNote(string noteIdentifier);
        Task<IResponse<NoteResourceResponse>> CreatePrivateNote(string notePath, string text);
    }
}