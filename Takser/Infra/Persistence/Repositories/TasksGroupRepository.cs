using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tasker.App.Persistence.Repositories;
using Tasker.Domain.Models;
using Tasker.Infra.Persistence.Context;

namespace Tasker.Infra.Persistence.Repositories
{
    public class TasksGroupRepository : IDbRepository<TasksGroup>
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
            return await mBaseRepository.Context.TasksGroups.Include(p => p.Tasks).ToListAsync();
        }

        public async Task<TasksGroup> FindByIdAsync(string id)
        {
            return await mBaseRepository.Context.TasksGroups.FindAsync(id);
        }

        public Task UpdateAsync(TasksGroup group)
        {
            mBaseRepository.Context.TasksGroups.Update(group);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(TasksGroup group)
        {
            mBaseRepository.Context.TasksGroups.Remove(group);
            return Task.CompletedTask;
        }
    }
}