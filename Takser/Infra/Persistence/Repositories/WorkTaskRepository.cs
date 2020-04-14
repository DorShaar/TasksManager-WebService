using System.Collections.Generic;
using System.Threading.Tasks;
using TaskData.Contracts;
using Tasker.App.Persistence.Repositories;
using Tasker.Infra.Persistence.Context;

namespace Tasker.Infra.Persistence.Repositories
{
    public class WorkTaskRepository : IDbRepository<IWorkTask>
    {
        private readonly AppDbContext mDatabase;

        public WorkTaskRepository()
        {
        }

        public Task AddAsync(IWorkTask workTask)
        {
            throw new System.NotImplementedException();
        }

        public Task<IWorkTask> FindByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<IWorkTask>> ListAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveAsync(IWorkTask workTask)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(IWorkTask workTask)
        {
            throw new System.NotImplementedException();
        }
    }
}