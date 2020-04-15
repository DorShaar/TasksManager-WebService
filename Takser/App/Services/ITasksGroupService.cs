using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskData.Contracts;
using Tasker.Domain.Communication;

namespace Tasker.App.Services
{
    public interface ITasksGroupService
    {
        Task<IEnumerable<ITasksGroup>> FindTasksGroupsByConditionAsync(Func<ITasksGroup, bool> condition);
        Task<IEnumerable<IWorkTask>> FindWorkTasksByTasksGroupConditionAsync(Func<ITasksGroup, bool> condition);
        Task<IEnumerable<ITasksGroup>> ListAsync();
        Task<Response<ITasksGroup>> SaveAsync(string groupName);
        Task<Response<IWorkTask>> SaveTaskAsync(string taskGroupIdentifier, string workTaskDescription);
        Task<Response<ITasksGroup>> UpdateAsync(string id, string newGroupName);
        Task<Response<ITasksGroup>> RemoveAsync(string id);
        Task<Response<IWorkTask>> RemoveTaskAsync(string id);
    }
}