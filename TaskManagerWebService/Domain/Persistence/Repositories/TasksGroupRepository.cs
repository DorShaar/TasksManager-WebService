using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyFirstWebApp.Domain.Models;
using MyFirstWebApp.Domain.Persistence.Context;
using MyFirstWebApp.Domain.Repositories;

namespace MyFirstWebApp.Domain.Persistence.Repositories
{
    public class TasksGroupRepository : BaseRepository, ITasksGroupRepository
    {
        public TasksGroupRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<TasksGroup>> ListAsync()
        {
            return await mContext.TasksGroups.ToListAsync();
        }
    }
}