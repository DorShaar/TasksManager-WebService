using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tasker.App.Persistence.Repositories;
using Tasker.Domain.Models;
using Tasker.Infra.Persistence.Context;
using Tasker.Infra.Persistence.Repositories;

namespace TaskManagerWebService.Persistence.Repositories
{
    public class WorkTaskRepository : IDbRepository<WorkTask>
    {
        private readonly BaseRepository mBaseRepository;

        public WorkTaskRepository(AppDbContext context)
        {
            mBaseRepository = new BaseRepository(context);
        }

        public Task AddAsync(WorkTask entity)
        {
            throw new System.NotImplementedException();
        }

        public Task<WorkTask> FindByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<WorkTask>> ListAsync()
        {
            return await mBaseRepository.Context.WorkTasks.Include(task => task.ParentGroup).ToListAsync();
        }

        public Task RemoveAsync(WorkTask entity)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(WorkTask entity)
        {
            throw new System.NotImplementedException();
        }
    }
}