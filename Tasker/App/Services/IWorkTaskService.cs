using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskData.WorkTasks;

namespace Tasker.App.Services
{
    public interface IWorkTaskService
    {
        Task<IEnumerable<IWorkTask>> FindWorkTasksByConditionAsync(Func<IWorkTask, bool> condition);
        Task<IEnumerable<IWorkTask>> ListAllAsync();
    }
}