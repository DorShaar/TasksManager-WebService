using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagerWebService.Domain.Models;
using TaskManagerWebService.Persistence.Context;
using TaskManagerWebService.Domain.Repositories;

namespace TaskManagerWebService.Persistence.Repositories
{
    public class TasksGroupRepository : ITasksGroupRepository
    {
        private readonly BaseRepository mBaseRepository;

        public TasksGroupRepository(AppDbContext context)
        {
            mBaseRepository = new BaseRepository(context);
        }

        public async Task AddAsync(TasksGroup group)
        {
            await mBaseRepository.Context.TasksGroups.AddAsync(group);
        }

        public async Task<IEnumerable<TasksGroup>> ListAsync()
        {
            return await mBaseRepository.Context.TasksGroups.ToListAsync();
        }

        public async Task<TasksGroup> FindByIdAsync(string id)
        {
            return await mBaseRepository.Context.TasksGroups.FindAsync(id);
        }

        public void Update(TasksGroup group)
        {
            mBaseRepository.Context.TasksGroups.Update(group);
        }
    }
}