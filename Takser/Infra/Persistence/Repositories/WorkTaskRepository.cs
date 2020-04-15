using Logger.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Takser.App.Persistence.Context;
using TaskData.Contracts;
using Tasker.App.Persistence.Repositories;

namespace Tasker.Infra.Persistence.Repositories
{
    public class WorkTaskRepository : IDbRepository<IWorkTask>
    {
        private readonly ILogger mLogger;
        private readonly IAppDbContext mDatabase;

        public WorkTaskRepository(IAppDbContext database, ILogger logger)
        {
            mDatabase = database;
            mLogger = logger;
        }

        public async Task AddAsync(IWorkTask workTask)
        {
            if (workTask == null)
            {
                mLogger.LogError($"Work task given is null, no database adding performed");
                return;
            }

            await mDatabase.SaveCurrentDatabase();
        }

        public async Task<IWorkTask> FindAsync(string workTaskId)
        {
            foreach (IWorkTask workTask in await ListAsync())
            {
                if (workTask.ID == workTaskId)
                    return workTask;
            }

            mLogger.LogError($"Task {workTaskId} was not found");
            return null;
        }

        public async Task<IEnumerable<IWorkTask>> ListAsync()
        {
            await mDatabase.LoadDatabase();

            List<IWorkTask> allTasks = new List<IWorkTask>();

            foreach (ITasksGroup taskGroup in mDatabase.Entities)
            {
                allTasks.AddRange(taskGroup.GetAllTasks());
            }

            return allTasks.AsEnumerable();
        }

        public async Task RemoveAsync(IWorkTask workTask)
        {
            if (workTask == null)
            {
                mLogger.LogError($"Work task given is null, no database removing performed");
                return;
            }

            await mDatabase.SaveCurrentDatabase();
        }

        public async Task UpdateAsync(IWorkTask workTask)
        {
            if (workTask == null)
            {
                mLogger.LogError($"Work task given is null, no database updaing performed");
                return;
            }

            await mDatabase.SaveCurrentDatabase();
        }
    }
}