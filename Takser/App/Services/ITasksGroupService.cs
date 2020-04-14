using System.Collections.Generic;
using System.Threading.Tasks;
using TaskData.Contracts;
using Tasker.Domain.Communication;

namespace Tasker.App.Services
{
    public interface ITasksGroupService
    {
        Task<IEnumerable<ITasksGroup>> ListAsync();
        Task<TasksGroupResponse> SaveAsync(ITasksGroup group);
        Task<TasksGroupResponse> UpdateAsync(string id, string newGroupName);
        Task<TasksGroupResponse> RemoveAsync(string id);
    }
}