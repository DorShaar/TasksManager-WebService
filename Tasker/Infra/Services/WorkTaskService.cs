using Logger.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskData.Contracts;
using Tasker.App.Persistence.Repositories;
using Tasker.App.Services;

namespace Tasker.Infra.Services
{
    public class WorkTaskService : IWorkTaskService
    {
        private readonly IDbRepository<IWorkTask> mWorkTaskRepository;
        private readonly ILogger mLogger;

        public WorkTaskService(IDbRepository<IWorkTask> workTaskRepository, ILogger logger)
        {
            mWorkTaskRepository = workTaskRepository;
            mLogger = logger;
        }

        public async Task<IEnumerable<IWorkTask>> FindWorkTasksByConditionAsync(Func<IWorkTask, bool> condition)
        {
            List<IWorkTask> workTasks = new List<IWorkTask>();

            foreach (IWorkTask task in await ListAsync())
            {
                if (condition(task))
                    workTasks.Add(task);
            }

            mLogger.Log($"Found {workTasks.Count} tasks");
            return workTasks;
        }

        public async Task<IEnumerable<IWorkTask>> ListAsync()
        {
            IEnumerable<IWorkTask> workTasks = await mWorkTaskRepository.ListAsync();

            if (workTasks == null)
                return new List<IWorkTask>();

            return workTasks;
        }
    }
}