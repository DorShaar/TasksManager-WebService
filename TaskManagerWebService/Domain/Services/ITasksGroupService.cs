using TaskManagerWebService.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerWebService.Domain.Services.Communication;

namespace TaskManagerWebService.Domain.Services
{
    public interface ITasksGroupService
    {
        Task<IEnumerable<TasksGroup>> ListAsync();
        Task<SaveTasksGroupResponse> SaveAsync(TasksGroup group);
    }
}