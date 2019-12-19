using MyFirstWebApp.Domain.Models;
using MyFirstWebApp.Domain.Repositories;
using MyFirstWebApp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFirstWebApp.Services
{
    public class TasksGroupService : ITasksGroupService
    {
        private readonly ITasksGroupRepository mTasksGroupRepository;

        public TasksGroupService(ITasksGroupRepository workTaskRepository)
        {
            mTasksGroupRepository = workTaskRepository;
        }

        public async Task<IEnumerable<TasksGroup>> ListAsync()
        {
            return await mTasksGroupRepository.ListAsync();
        }
    }
}