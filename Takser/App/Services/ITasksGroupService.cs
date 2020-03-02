using System.Collections.Generic;
using System.Threading.Tasks;
using Tasker.Domain.Communication;
using Tasker.Domain.Models;

namespace Tasker.App.Services
{
    public interface ITasksGroupService
    {
        Task<IEnumerable<TasksGroup>> ListAsync();
        Task<TasksGroupResponse> SaveAsync(TasksGroup group);
        Task<TasksGroupResponse> UpdateAsync(string id, TasksGroup group);
        Task<TasksGroupResponse> RemoveAsync(string id);
    }
}