using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
using Tasker.App.Resources;
using Tasker.Domain.Communication;

namespace Tasker.App.Services
{
    public interface ITasksGroupService
    {
        Task<IEnumerable<ITasksGroup>> FindTasksGroupsByConditionAsync(Func<ITasksGroup, bool> condition);
        Task<IEnumerable<IWorkTask>> FindWorkTasksByTasksGroupConditionAsync(Func<ITasksGroup, bool> condition);
        Task<IEnumerable<ITasksGroup>> ListAsync();
        Task<IResponse<ITasksGroup>> SaveAsync(string groupName);
        Task<IResponse<IWorkTask>> SaveTaskAsync(string taskGroupIdentifier, string workTaskDescription);
        Task<IResponse<ITasksGroup>> UpdateGroupAsync(string id, string newGroupName);
        Task<IResponse<IWorkTask>> UpdateTaskAsync(WorkTaskResource workTaskResource);
        Task<IResponse<IWorkTask>> MoveTaskAsync(string taskId, string tasksGroupId);
        Task<IResponse<ITasksGroup>> RemoveAsync(string id);
        Task<IResponse<IWorkTask>> RemoveTaskAsync(string id);
    }
}