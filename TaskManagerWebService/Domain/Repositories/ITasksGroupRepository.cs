using MyFirstWebApp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFirstWebApp.Domain.Repositories
{
    public interface ITasksGroupRepository
    {
        Task<IEnumerable<TasksGroup>> ListAsync();
    }
}