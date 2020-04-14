using System.Collections.Generic;
using System.Threading.Tasks;
using Takser.Domain.Communication;
using TaskData.Contracts;

namespace Tasker.App.Services
{
    public interface IWorkTaskService
    {
        Task<IEnumerable<IWorkTask>> ListAsync();
        Task<WorkTaskResponse> SaveAsync(IWorkTask group);
        Task<WorkTaskResponse> UpdateAsync(string id, IWorkTask group);
        Task<WorkTaskResponse> RemoveAsync(string id);
    }
}