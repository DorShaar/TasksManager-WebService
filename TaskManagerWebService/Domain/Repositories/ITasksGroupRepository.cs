using TaskManagerWebService.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskManagerWebService.Domain.Repositories
{
    public interface ITasksGroupRepository
    {
        Task<IEnumerable<TasksGroup>> ListAsync();
        Task AddAsync(TasksGroup group);
    }
}