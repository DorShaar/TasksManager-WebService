using MyFirstWebApp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFirstWebApp.Domain.Services
{
    public interface ITasksGroupService
    {
        Task<IEnumerable<TasksGroup>> ListAsync();
    }
}