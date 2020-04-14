using System.Collections.Generic;
using System.Threading.Tasks;
using TaskData.Contracts;
using Tasker.Domain.Communication;

namespace Tasker.App.Services
{
    public interface IWorkTaskService
    {
        Task<IEnumerable<IWorkTask>> ListAsync();
        Task<Response<IWorkTask>> SaveAsync(IWorkTask group);
        Task<Response<IWorkTask>> UpdateAsync(string id, IWorkTask group);
        Task<Response<IWorkTask>> RemoveAsync(string id);
    }
}