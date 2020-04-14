using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskData.Contracts;
using Tasker.App.Persistence.Repositories;
using Tasker.Infra.Persistence.Context;

namespace Tasker.Infra.Persistence.Repositories
{
    public class TasksGroupRepository : IDbRepository<ITasksGroup>
    {
        private readonly AppDbContext mDatabase;

        public TasksGroupRepository(AppDbContext database)
        {
            mDatabase = database;
        }

        public async Task AddAsync(ITasksGroup group)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ITasksGroup>> ListAsync()
        {
            throw new NotImplementedException();
            // TODO
        }

        public async Task<ITasksGroup> FindByIdAsync(string id)
        {
            throw new NotImplementedException();
            // TODO
        }

        public Task UpdateAsync(ITasksGroup group)
        {
            // TODO
            return Task.CompletedTask;
        }

        public Task RemoveAsync(ITasksGroup group)
        {
            // TODO
            return Task.CompletedTask;
        }
    }
}