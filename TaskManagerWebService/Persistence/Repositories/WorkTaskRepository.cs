using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerWebService.Domain.Models;
using TaskManagerWebService.Domain.Repositories;
using TaskManagerWebService.Persistence.Context;

namespace TaskManagerWebService.Persistence.Repositories
{
    public class WorkTaskRepository : IWorkTaskRepository
    {
        private readonly BaseRepository mBaseRepository;

        public WorkTaskRepository(AppDbContext context)
        {
            mBaseRepository = new BaseRepository(context);
        }

        public async Task<IEnumerable<WorkTask>> ListAsync()
        {
            return await mBaseRepository.Context.WorkTasks.Include(task => task.ParentGroup).ToListAsync();
        }
    }
}