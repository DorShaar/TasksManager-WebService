using TaskManagerWebService.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerWebService.Domain.Services.Communication;

namespace TaskManagerWebService.Domain.Services
{
    public interface ITasksGroupService
    {
        Task<IEnumerable<TasksGroup>> ListAsync();
        Task<TasksGroupResponse> SaveAsync(TasksGroup group);
        Task<TasksGroupResponse> UpdateAsync(string id, TasksGroup group);
        Task<TasksGroupResponse> RemoveAsync(string id);
    }
}