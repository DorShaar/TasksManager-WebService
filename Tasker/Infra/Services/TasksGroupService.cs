using Logger.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskData.Contracts;
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
        private readonly ITasksGroupBuilder mTaskGroupBuilder;
        private readonly NameValidator mTasksGroupNameValidator;
        private readonly NameValidator mWorkTaskNameValidator;
        private readonly ILogger mLogger;

        public TasksGroupService(IDbRepository<ITasksGroup> TaskGroupRepository, ITasksGroupBuilder tasksGroupBuilder, ILogger logger)
        {
            mTasksGroupRepository = TaskGroupRepository;
            mTaskGroupBuilder = tasksGroupBuilder;
            mLogger = logger;
            mTasksGroupNameValidator = new NameValidator(NameLengths.MaximalGroupNameLength);
            mWorkTaskNameValidator = new NameValidator(NameLengths.MaximalTaskNameLength);
        }

        public async Task<IEnumerable<ITasksGroup>> FindTasksGroupsByConditionAsync(Func<ITasksGroup, bool> condition)
        {
            List<ITasksGroup> tasksGroups = new List<ITasksGroup>();

            foreach (ITasksGroup taskGroup in await ListAsync())
            {
                if (condition(taskGroup))
                    tasksGroups.Add(taskGroup);
            }

            mLogger.Log($"Found {tasksGroups.Count} tasks");
            return tasksGroups;
        }

        public async Task<IEnumerable<IWorkTask>> FindWorkTasksByTasksGroupConditionAsync(Func<ITasksGroup, bool> condition)
        {
            List<IWorkTask> workTasks = new List<IWorkTask>();

            foreach (ITasksGroup taskGroup in await ListAsync())
            {
                if (condition(taskGroup))
                    workTasks.AddRange(taskGroup.GetAllTasks());
            }

            mLogger.Log($"Found {workTasks.Count} tasks");
            return workTasks;
        }

        public async Task<IEnumerable<ITasksGroup>> ListAsync()
        {
            IEnumerable<ITasksGroup> tasksGroups = await mTasksGroupRepository.ListAsync();

            if (tasksGroups == null)
                return new List<ITasksGroup>();

            return tasksGroups;
        }

        public async Task<IResponse<ITasksGroup>> SaveAsync(string groupName)
        {
            try
            {
                ITasksGroup tasksGroup = mTaskGroupBuilder.Create(groupName, mLogger);

                if (!mTasksGroupNameValidator.IsNameValid(tasksGroup.Name))
                {
                    return new FailResponse<ITasksGroup>(
                        $"Group name '{tasksGroup.Name}' exceeds the maximal group name length: {NameLengths.MaximalGroupNameLength}");
                }

                await mTasksGroupRepository.AddAsync(tasksGroup);

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
                    (await FindTasksGroupsByConditionAsync(group => group.ID == taskGroupIdentifier)).FirstOrDefault();
                if (tasksGroup == null && taskGroupIdentifier != null)
                {
                    tasksGroup = (await FindTasksGroupsByConditionAsync(
                        group => group.Name.ToLower() == taskGroupIdentifier.ToLower())).FirstOrDefault();
                }

                if (tasksGroup == null)
                    return new FailResponse<IWorkTask>($"Tasks group {taskGroupIdentifier} does not exist");

                if (!ValidateUniqueTaskDescription(tasksGroup, workTaskDescription))
                    return new FailResponse<IWorkTask>($"Tasks group {tasksGroup.Name} has already work task with description {workTaskDescription}");

                IWorkTask workTask = tasksGroup.CreateTask(workTaskDescription);

                if (!mWorkTaskNameValidator.IsNameValid(workTask.Description))
                    return new FailResponse<IWorkTask>($"Task description'{workTask.Description}' is invalid");

                await mTasksGroupRepository.UpdateAsync(tasksGroup);

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

            if (await IsTasksGroupNameAlreadyExist(newGroupName))
                return new FailResponse<ITasksGroup>($"Group name '{newGroupName}' is already exists");

            ITasksGroup groupToUpdate = await mTasksGroupRepository.FindAsync(id);

            if (groupToUpdate == null)
                return new FailResponse<ITasksGroup>("Group not found");

            try
            {
                groupToUpdate.Name = newGroupName;
                await UpdateGroupNamesForAllChildren(groupToUpdate);

                await mTasksGroupRepository.UpdateAsync(groupToUpdate);

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
                (await ListAsync()).Select(group => group.Name)
                                   .Where(groupName => groupName.ToLower()
                                   .Equals(groupNameToCheck.ToLower()));

            return identicalNames.Any();
        }

        private Task UpdateGroupNamesForAllChildren(ITasksGroup tasksGroup)
        {
            foreach (IWorkTask workTaskChild in tasksGroup.GetAllTasks())
            {
                mLogger.Log($"Updating task's group name from {workTaskChild.GroupName} to {tasksGroup.Name}");
                workTaskChild.GroupName = tasksGroup.Name;
            }

            return Task.CompletedTask;
        }

        public async Task<IResponse<IWorkTask>> UpdateTaskAsync(WorkTaskResource workTaskResource)
        {
            try
            {
                if (string.IsNullOrEmpty(workTaskResource.TaskId))
                    return new FailResponse<IWorkTask>($"Task id is invalid (null or empty)");

                if (!string.IsNullOrEmpty(workTaskResource.Description))
                    return await UpdateDescription(workTaskResource.TaskId, workTaskResource.Description);

                if (!string.IsNullOrEmpty(workTaskResource.GroupName))
                    return await MoveTaskAsync(workTaskResource.TaskId, workTaskResource.GroupName);

                return new FailResponse<IWorkTask>($"Work task resource does not contain update information as " +
                    $"description or group name");
            }
            catch (Exception ex)
            {
                return new FailResponse<IWorkTask>(
                    $"An error occurred when updating work task id {workTaskResource.TaskId}: {ex.Message}");
            }
        }

        private async Task <IResponse<IWorkTask>> UpdateDescription(string workTaskId, string newTaskDescription)
        {
            if (!mWorkTaskNameValidator.IsNameValid(newTaskDescription))
                return new FailResponse<IWorkTask>($"Task description'{newTaskDescription}' is invalid");

            (ITasksGroup groupParent, IWorkTask taskToUpdate) = await FindWorkTaskAndItsParentGroup(workTaskId);

            if (taskToUpdate != null)
            {
                if (IsWorkTaskDescriptionAlreadyExist(groupParent, newTaskDescription))
                {
                    return new FailResponse<IWorkTask>(
                        $"Task description'{newTaskDescription}' is already exist in group {groupParent.ID}");
                }

                taskToUpdate.Description = newTaskDescription;
                groupParent.UpdateTask(taskToUpdate);
                await mTasksGroupRepository.UpdateAsync(groupParent);

                return new SuccessResponse<IWorkTask>(taskToUpdate);
            }

            return new FailResponse<IWorkTask>($"Work task {workTaskId} not found. No task update performed");
        }

        public async Task<IResponse<IWorkTask>> MoveTaskAsync(string workTaskId, string tasksGroupId)
        {
            ITasksGroup destinationGroup = await mTasksGroupRepository.FindAsync(tasksGroupId);
            if (destinationGroup == null)
                return new FailResponse<IWorkTask>($"Entity group {tasksGroupId} not found. No move performed");

            try
            {
                (ITasksGroup OldParentGroup, IWorkTask taskToMove) = await FindWorkTaskAndItsParentGroup(workTaskId);

                if (taskToMove != null)
                {
                    if (IsWorkTaskDescriptionAlreadyExist(destinationGroup, taskToMove.Description))
                    {
                        return new FailResponse<IWorkTask>(
                            $"Task description'{taskToMove.Description}' is already exist in group {destinationGroup.ID}");
                    }

                    destinationGroup.AddTask(taskToMove);
                    await mTasksGroupRepository.UpdateAsync(destinationGroup);

                    OldParentGroup.RemoveTask(taskToMove.ID);
                    await mTasksGroupRepository.UpdateAsync(OldParentGroup);

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
            return tasksGroup.GetAllTasks().FirstOrDefault(
                task => task.Description.ToLower().Equals(workTaskDescription.ToLower())) != null;
        }

        public async Task<IResponse<ITasksGroup>> RemoveAsync(string id)
        {
            ITasksGroup groupToRemove = await mTasksGroupRepository.FindAsync(id);

            if (groupToRemove == null)
                return new FailResponse<ITasksGroup>($"Entity group {id} not found. No deletion performed");

            if (groupToRemove.Size > 0)
            {
                StringBuilder idsInGroup = new StringBuilder();
                groupToRemove.GetAllTasks().Select(task => task.ID).ToList().ForEach(id => idsInGroup.Append($"{id}, "));
                return new FailResponse<ITasksGroup>(groupToRemove, $"Entity group {id} cannot be deleted. Please move or remove" +
                        $" the next work tasks ids: {idsInGroup}");
            }

            try
            {
                await mTasksGroupRepository.RemoveAsync(groupToRemove);

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
                (ITasksGroup groupParent, IWorkTask taskToRemove) = await FindWorkTaskAndItsParentGroup(workTaskId);

                if (taskToRemove != null)
                {
                    groupParent.RemoveTask(workTaskId);
                    await mTasksGroupRepository.UpdateAsync(groupParent);

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
            foreach (ITasksGroup group in await ListAsync())
            {
                IWorkTask task = group.GetTask(workTaskId);
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