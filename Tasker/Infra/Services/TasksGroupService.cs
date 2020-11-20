using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskData.OperationResults;
using TaskData.TasksGroups;
using TaskData.TaskStatus;
using TaskData.WorkTasks;
using Tasker.App.Persistence.Repositories;
using Tasker.App.Resources;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Tasker.Domain.Options;
using Tasker.Domain.Validators;

namespace Tasker.Infra.Services
{
    public class TasksGroupService : ITasksGroupService
    {
        private readonly IDbRepository<ITasksGroup> mTasksGroupRepository;
        private readonly ITasksGroupFactory mTaskGroupFactory;
        private readonly NameValidator mTasksGroupNameValidator;
        private readonly NameValidator mWorkTaskNameValidator;
        private readonly ILogger<TasksGroupService> mLogger;

        public TasksGroupService(IDbRepository<ITasksGroup> TaskGroupRepository,
            ITasksGroupFactory tasksGroupBuilder,
            ILogger<TasksGroupService> logger)
        {
            mTasksGroupRepository = TaskGroupRepository ?? throw new ArgumentNullException(nameof(TaskGroupRepository));
            mTaskGroupFactory = tasksGroupBuilder ?? throw new ArgumentNullException(nameof(tasksGroupBuilder));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));

            mTasksGroupNameValidator = new NameValidator(NameLengths.MaximalGroupNameLength);
            mWorkTaskNameValidator = new NameValidator(NameLengths.MaximalTaskNameLength);
        }

        public async Task<IEnumerable<ITasksGroup>> FindTasksGroupsByConditionAsync(Func<ITasksGroup, bool> condition)
        {
            List<ITasksGroup> tasksGroups = new List<ITasksGroup>();

            foreach (ITasksGroup taskGroup in await ListAsync().ConfigureAwait(false))
            {
                if (condition(taskGroup))
                    tasksGroups.Add(taskGroup);
            }

            mLogger.LogDebug($"Found {tasksGroups.Count} tasks");
            return tasksGroups;
        }

        public async Task<IEnumerable<IWorkTask>> FindWorkTasksByTasksGroupConditionAsync(
            Func<ITasksGroup, bool> condition)
        {
            List<IWorkTask> workTasks = new List<IWorkTask>();

            foreach (ITasksGroup taskGroup in await ListAsync().ConfigureAwait(false))
            {
                if (condition(taskGroup))
                    workTasks.AddRange(taskGroup.GetAllTasks());
            }

            mLogger.LogDebug($"Found {workTasks.Count} tasks");
            return workTasks;
        }

        public async Task<IEnumerable<ITasksGroup>> ListAsync()
        {
            IEnumerable<ITasksGroup> tasksGroups = await mTasksGroupRepository.ListAsync().ConfigureAwait(false);

            return tasksGroups ?? new List<ITasksGroup>();
        }

        public async Task<IResponse<ITasksGroup>> SaveAsync(string groupName)
        {
            try
            {
                ITasksGroup tasksGroup = mTaskGroupFactory.CreateGroup(groupName);

                if (!mTasksGroupNameValidator.IsNameValid(tasksGroup.Name))
                {
                    return new FailResponse<ITasksGroup>(
                        $"Group name '{tasksGroup.Name}' exceeds the maximal group name length: {NameLengths.MaximalGroupNameLength}");
                }

                if (!await mTasksGroupRepository.AddAsync(tasksGroup).ConfigureAwait(false))
                {
                    return new FailResponse<ITasksGroup>(
                        $"Group name '{tasksGroup.Name}' already exist");
                }

                return new SuccessResponse<ITasksGroup>(tasksGroup);
            }
            catch (Exception ex)
            {
                return new FailResponse<ITasksGroup>($"An error occurred when saving tasks group name {groupName}: {ex.Message}");
            }
        }

        public async Task<IResponse<IWorkTask>> SaveTaskAsync(string taskGroupIdentifier, string workTaskDescription)
        {
            try
            {
                ITasksGroup tasksGroup =
                    (await FindTasksGroupsByConditionAsync(group => group.ID == taskGroupIdentifier)
                    .ConfigureAwait(false))
                    .FirstOrDefault();

                if (tasksGroup == null && taskGroupIdentifier != null)
                {
                    tasksGroup = (await FindTasksGroupsByConditionAsync(group =>
                        string.Equals(group.Name, taskGroupIdentifier, StringComparison.OrdinalIgnoreCase))
                        .ConfigureAwait(false))
                        .FirstOrDefault();
                }

                if (tasksGroup == null)
                    return new FailResponse<IWorkTask>($"Tasks group {taskGroupIdentifier} does not exist");

                if (!ValidateUniqueTaskDescription(tasksGroup, workTaskDescription))
                {
                    return new FailResponse<IWorkTask>($"Tasks group {tasksGroup.Name} " +
                        $"has already work task with description {workTaskDescription}");
                }

                IWorkTask workTask = mTaskGroupFactory.CreateTask(tasksGroup, workTaskDescription);

                if (!mWorkTaskNameValidator.IsNameValid(workTask.Description))
                    return new FailResponse<IWorkTask>($"Task description'{workTask.Description}' is invalid");

                await mTasksGroupRepository.UpdateAsync(tasksGroup).ConfigureAwait(false);

                return new SuccessResponse<IWorkTask>(workTask);
            }
            catch (Exception ex)
            {
                return new FailResponse<IWorkTask>($"An error occurred when saving work task {taskGroupIdentifier}: {ex.Message}");
            }
        }

        public async Task<IResponse<ITasksGroup>> UpdateGroupAsync(string id, string newGroupName)
        {
            if (!mTasksGroupNameValidator.IsNameValid(newGroupName))
                return new FailResponse<ITasksGroup>($"Group name '{newGroupName}' is invalid");

            if (await IsTasksGroupNameAlreadyExist(newGroupName).ConfigureAwait(false))
                return new FailResponse<ITasksGroup>($"Group name '{newGroupName}' is already exists");

            ITasksGroup groupToUpdate = await mTasksGroupRepository.FindAsync(id).ConfigureAwait(false);

            if (groupToUpdate == null)
                return new FailResponse<ITasksGroup>("Group not found");

            try
            {
                OperationResult setGroupNameResult = groupToUpdate.SetGroupName(newGroupName);

                if (!setGroupNameResult.Success)
                    return new FailResponse<ITasksGroup>(setGroupNameResult.Reason);

                await UpdateGroupNamesForAllChildren(groupToUpdate).ConfigureAwait(false);

                await mTasksGroupRepository.UpdateAsync(groupToUpdate).ConfigureAwait(false);

                return new SuccessResponse<ITasksGroup>(groupToUpdate);
            }
            catch (Exception ex)
            {
                return new FailResponse<ITasksGroup>($"An error occurred when updating tasks group id {id}: {ex.Message}");
            }
        }

        private async Task<bool> IsTasksGroupNameAlreadyExist(string groupNameToCheck)
        {
            IEnumerable<string> identicalNames =
                (await ListAsync().ConfigureAwait(false)).Select(group => group.Name)
                                   .Where(groupName => groupName.Equals(groupNameToCheck, StringComparison.OrdinalIgnoreCase));

            return identicalNames.Any();
        }

        private Task UpdateGroupNamesForAllChildren(ITasksGroup tasksGroup)
        {
            foreach (IWorkTask workTaskChild in tasksGroup.GetAllTasks())
            {
                mLogger.LogDebug($"Updating task's group name from {workTaskChild.GroupName} to {tasksGroup.Name}");
                workTaskChild.GroupName = tasksGroup.Name;
            }

            return Task.CompletedTask;
        }

        public async Task<IResponse<IWorkTask>> UpdateTaskAsync(WorkTaskResource workTaskResource)
        {
            try
            {
                if (string.IsNullOrEmpty(workTaskResource.TaskId))
                {
                    return new FailResponse<IWorkTask>("Task id is invalid (null or empty)");
                }

                if (!string.IsNullOrEmpty(workTaskResource.Description))
                {
                    return await UpdateDescriptionAsync(workTaskResource.TaskId, workTaskResource.Description)
                        .ConfigureAwait(false);
                }

                if (!string.IsNullOrEmpty(workTaskResource.Status))
                {
                    return await UpdateStatusAsync(workTaskResource.TaskId, workTaskResource.Status, workTaskResource.Reason)
                        .ConfigureAwait(false);
                }

                if (!string.IsNullOrEmpty(workTaskResource.GroupName))
                {
                    return await MoveTaskAsync(workTaskResource.TaskId, workTaskResource.GroupName)
                        .ConfigureAwait(false);
                }

                return new FailResponse<IWorkTask>("Work task resource does not contain update information as " +
                    "description or group name");
            }
            catch (Exception ex)
            {
                return new FailResponse<IWorkTask>(
                    $"An error occurred when updating work task id {workTaskResource.TaskId}: {ex.Message}");
            }
        }

        private async Task<IResponse<IWorkTask>> UpdateDescriptionAsync(string workTaskId, string newTaskDescription)
        {
            if (!mWorkTaskNameValidator.IsNameValid(newTaskDescription))
                return new FailResponse<IWorkTask>($"Task description'{newTaskDescription}' is invalid");

            (ITasksGroup groupParent, IWorkTask taskToUpdate) =
                await FindWorkTaskAndItsParentGroup(workTaskId).ConfigureAwait(false);

            if (taskToUpdate != null)
            {
                if (IsWorkTaskDescriptionAlreadyExist(groupParent, newTaskDescription))
                {
                    return new FailResponse<IWorkTask>(
                        $"Task description'{newTaskDescription}' is already exist in group {groupParent.ID}");
                }

                taskToUpdate.Description = newTaskDescription;
                groupParent.UpdateTask(taskToUpdate);
                await mTasksGroupRepository.UpdateAsync(groupParent).ConfigureAwait(false);

                return new SuccessResponse<IWorkTask>(taskToUpdate);
            }

            return new FailResponse<IWorkTask>($"Work task {workTaskId} not found. No task update performed");
        }

        private async Task<IResponse<IWorkTask>> UpdateStatusAsync(string workTaskId, string newTaskStatus, string reason)
        {
            if (!Enum.TryParse(newTaskStatus, out Status newStatus))
                return new FailResponse<IWorkTask>($"Failed to parse status '{newTaskStatus}'");

            (ITasksGroup groupParent, IWorkTask taskToUpdate) =
                await FindWorkTaskAndItsParentGroup(workTaskId).ConfigureAwait(false);

            if (taskToUpdate == null)
                return new FailResponse<IWorkTask>($"Work task {workTaskId} not found. No task update performed");

            if (newStatus == taskToUpdate.Status)
                return new FailResponse<IWorkTask>($"Task id {workTaskId} has already status'{newTaskStatus}'");

            switch (newStatus)
            {
                case Status.Closed:
                    taskToUpdate.CloseTask(reason);
                    break;

                case Status.OnWork:
                    taskToUpdate.MarkTaskOnWork(reason);
                    break;

                case Status.Open:
                    taskToUpdate.ReOpenTask(reason);
                    break;

                default:
                    return new FailResponse<IWorkTask>($"New status'{newTaskStatus}' is not expected");
            }

            groupParent.UpdateTask(taskToUpdate);
            await mTasksGroupRepository.UpdateAsync(groupParent).ConfigureAwait(false);
            return new SuccessResponse<IWorkTask>(taskToUpdate);
        }

        public async Task<IResponse<IWorkTask>> MoveTaskAsync(string workTaskId, string tasksGroupId)
        {
            ITasksGroup destinationGroup = await mTasksGroupRepository.FindAsync(tasksGroupId).ConfigureAwait(false);

            if (destinationGroup == null)
                return new FailResponse<IWorkTask>($"Entity group {tasksGroupId} not found. No move performed");

            try
            {
                (ITasksGroup OldParentGroup, IWorkTask taskToMove) =
                    await FindWorkTaskAndItsParentGroup(workTaskId).ConfigureAwait(false);

                if (taskToMove != null)
                {
                    if (IsWorkTaskDescriptionAlreadyExist(destinationGroup, taskToMove.Description))
                    {
                        return new FailResponse<IWorkTask>(
                            $"Task description'{taskToMove.Description}' is already exist in group {destinationGroup.ID}");
                    }

                    destinationGroup.AddTask(taskToMove);
                    await mTasksGroupRepository.UpdateAsync(destinationGroup).ConfigureAwait(false);

                    OldParentGroup.RemoveTask(taskToMove.ID);
                    await mTasksGroupRepository.UpdateAsync(OldParentGroup).ConfigureAwait(false);

                    return new SuccessResponse<IWorkTask>(taskToMove);
                }

                return new FailResponse<IWorkTask>($"Work task {workTaskId} not found. No task move performed");
            }
            catch (Exception ex)
            {
                return new FailResponse<IWorkTask>($"An error occurred when moving task id {workTaskId}: {ex.Message}");
            }
        }

        private bool IsWorkTaskDescriptionAlreadyExist(ITasksGroup tasksGroup, string workTaskDescription)
        {
            return tasksGroup.GetAllTasks().Any(
                task => task.Description.Equals(workTaskDescription, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IResponse<ITasksGroup>> RemoveAsync(string id)
        {
            ITasksGroup groupToRemove = await mTasksGroupRepository.FindAsync(id).ConfigureAwait(false);

            if (groupToRemove == null)
                return new FailResponse<ITasksGroup>($"Entity group {id} not found. No deletion performed");

            if (groupToRemove.Size > 0)
            {
                StringBuilder idsInGroup = new StringBuilder();
                groupToRemove.GetAllTasks().Select(task => task.ID).ToList().ForEach(id => idsInGroup.Append(id).Append(", "));
                return new FailResponse<ITasksGroup>(groupToRemove, $"Entity group {id} cannot be deleted. Please move or remove" +
                        $" the next work tasks ids: {idsInGroup}");
            }

            try
            {
                await mTasksGroupRepository.RemoveAsync(groupToRemove).ConfigureAwait(false);

                return new SuccessResponse<ITasksGroup>(groupToRemove);
            }
            catch (Exception ex)
            {
                return new FailResponse<ITasksGroup>($"An error occurred when removing tasks group id {id}: {ex.Message}");
            }
        }

        public async Task<IResponse<IWorkTask>> RemoveTaskAsync(string workTaskId)
        {
            try
            {
                (ITasksGroup groupParent, IWorkTask taskToRemove) =
                    await FindWorkTaskAndItsParentGroup(workTaskId).ConfigureAwait(false);

                if (taskToRemove != null)
                {
                    groupParent.RemoveTask(workTaskId);
                    await mTasksGroupRepository.UpdateAsync(groupParent).ConfigureAwait(false);

                    return new SuccessResponse<IWorkTask>(taskToRemove);
                }

                return new FailResponse<IWorkTask>($"Work task {workTaskId} not found. No task deletion performed");
            }
            catch (Exception ex)
            {
                return new FailResponse<IWorkTask>($"An error occurred when removing work task id {workTaskId}: {ex.Message}");
            }
        }

        private async Task<(ITasksGroup, IWorkTask)> FindWorkTaskAndItsParentGroup(string workTaskId)
        {
            foreach (ITasksGroup group in await ListAsync().ConfigureAwait(false))
            {
                IWorkTask task = group.GetTask(workTaskId).Value;
                if (task != null)
                    return (group, task);
            }

            return (null, null);
        }

        private bool ValidateUniqueTaskDescription(ITasksGroup tasksGroup, string workTaskDescription)
        {
            foreach (IWorkTask workTask in tasksGroup.GetAllTasks())
            {
                if (workTask.Description.Equals(workTaskDescription))
                    return false;
            }

            return true;
        }
    }
}