using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskData.WorkTasks;
using Tasker.App.Persistence.Repositories;
using Tasker.App.Services;

namespace Tasker.Infra.Services
{
    public class WorkTaskService : IWorkTaskService
    {
        private readonly IDbRepository<IWorkTask> mWorkTaskRepository;
        private readonly ILogger<WorkTaskService> mLogger;

        public WorkTaskService(IDbRepository<IWorkTask> workTaskRepository, ILogger<WorkTaskService> logger)
        {
            mWorkTaskRepository = workTaskRepository ?? throw new ArgumentNullException(nameof(workTaskRepository));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<IWorkTask>> FindWorkTasksByConditionAsync(Func<IWorkTask, bool> condition)
        {
            List<IWorkTask> workTasks = new List<IWorkTask>();

            foreach (IWorkTask task in await ListAllAsync().ConfigureAwait(false))
            {
                if (condition(task))
                    workTasks.Add(task);
            }

            mLogger.LogDebug($"Found {workTasks.Count} tasks");
            return workTasks;
        }

        public async Task<IEnumerable<IWorkTask>> ListAllAsync()
        {
            IEnumerable<IWorkTask> workTasks = await mWorkTaskRepository.ListAsync().ConfigureAwait(false);

            return workTasks ?? new List<IWorkTask>();
        }
    }
}