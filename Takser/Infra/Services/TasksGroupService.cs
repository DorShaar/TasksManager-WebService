using Logger.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskData.Contracts;
using Tasker.App.Persistence.Repositories;
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

        public async Task<Response<ITasksGroup>> SaveAsync(string groupName)
        {
            try
            {
                ITasksGroup tasksGroup = mTaskGroupBuilder.Create(groupName, mLogger);

                if (!mTasksGroupNameValidator.IsNameValid(tasksGroup.Name))
                    return new Response<ITasksGroup>(isSuccess: false, $"Group name '{tasksGroup.Name}' exceeds the maximal group name length: {NameLengths.MaximalGroupNameLength}");

                await mTasksGroupRepository.AddAsync(tasksGroup);

                return new Response<ITasksGroup>(tasksGroup, isSuccess: true);
            }
            catch (Exception ex)
            {
                return new Response<ITasksGroup>(isSuccess: false, $"An error occurred when saving tasks group name {groupName}: {ex.Message}");
            }
        }

        public async Task<Response<IWorkTask>> SaveTaskAsync(string taskGroupIdentifier, string workTaskDescription)
        {
            try
            {
                ITasksGroup tasksGroup = (await FindTasksGroupsByConditionAsync(group => group.ID == taskGroupIdentifier)).FirstOrDefault();
                if (tasksGroup == null)
                    tasksGroup = (await FindTasksGroupsByConditionAsync(group => group.Name == taskGroupIdentifier)).FirstOrDefault();

                if (tasksGroup == null)
                    return new Response<IWorkTask>(isSuccess: false, $"Tasks group {taskGroupIdentifier} does not exist");

                if (!ValidateUniqueTaskDescription(tasksGroup, workTaskDescription))
                    return new Response<IWorkTask>(isSuccess: false, $"Tasks group {tasksGroup.Name} has already work task with description {workTaskDescription}");

                IWorkTask workTask = tasksGroup.CreateTask(workTaskDescription);

                if (!mWorkTaskNameValidator.IsNameValid(workTask.Description))
                    return new Response<IWorkTask>(isSuccess: false, $"Group name '{workTask.Description}' is invalid");

                await mTasksGroupRepository.UpdateAsync(tasksGroup);

                return new Response<IWorkTask>(workTask, isSuccess: true);
            }
            catch (Exception ex)
            {
                return new Response<IWorkTask>(isSuccess: false, $"An error occurred when saving work task {taskGroupIdentifier}: {ex.Message}");
            }
        }

        public async Task<Response<ITasksGroup>> UpdateAsync(string id, string newGroupName)
        {
            ITasksGroup groupToUpdate = await mTasksGroupRepository.FindAsync(id);

            if (groupToUpdate == null)
                return new Response<ITasksGroup>(isSuccess: false, "Group not found");

            if (!mTasksGroupNameValidator.IsNameValid(newGroupName))
                return new Response<ITasksGroup>(isSuccess: false, $"Group name '{newGroupName}' is invalid");

            groupToUpdate.Name = newGroupName;

            try
            {
                await mTasksGroupRepository.UpdateAsync(groupToUpdate);

                return new Response<ITasksGroup>(groupToUpdate, isSuccess: true);
            }
            catch (Exception ex)
            {
                return new Response<ITasksGroup>(isSuccess: false, $"An error occurred when updating tasks group id {id}: {ex.Message}");
            }
        }

        public async Task<Response<ITasksGroup>> RemoveAsync(string id)
        {
            ITasksGroup groupToRemove = await mTasksGroupRepository.FindAsync(id);

            if (groupToRemove == null)
                return new Response<ITasksGroup>(isSuccess: false, $"Entity group {id} not found. No deletion performed");

            if (groupToRemove.Size > 0)
            {
                StringBuilder idsInGroup = new StringBuilder();
                groupToRemove.GetAllTasks().Select(task => task.ID).ToList().ForEach(id => idsInGroup.Append($"{id}, "));
                return new Response<ITasksGroup>(groupToRemove, isSuccess: false, $"Entity group {id} cannot be deleted. Please move or remove" +
                        $"the next work tasks ids: {idsInGroup}");
            }

            try
            {
                await mTasksGroupRepository.RemoveAsync(groupToRemove);

                return new Response<ITasksGroup>(groupToRemove, isSuccess: true);
            }
            catch (Exception ex)
            {
                return new Response<ITasksGroup>(isSuccess: false, $"An error occurred when removing tasks group id {id}: {ex.Message}");
            }
        }

        public async Task<Response<IWorkTask>> RemoveTaskAsync(string workTaskId)
        {
            try
            {
                foreach (ITasksGroup group in await ListAsync())
                {
                    IWorkTask taskToRemove = group.GetTask(workTaskId);
                    if (taskToRemove != null)
                    {
                        group.RemoveTask(workTaskId);
                        await mTasksGroupRepository.UpdateAsync(group);

                        return new Response<IWorkTask>(taskToRemove, isSuccess: true);
                    }
                }

                return new Response<IWorkTask>(isSuccess: false, $"Work task {workTaskId} not found. No task deletion performed");
            }
            catch (Exception ex)
            {
                return new Response<IWorkTask>(isSuccess: false, $"An error occurred when removing work task id {workTaskId}: {ex.Message}");
            }
        }

        private bool ValidateUniqueTaskDescription(ITasksGroup tasksGroup, string workTaskDescription)
        {
            foreach(IWorkTask workTask in tasksGroup.GetAllTasks())
            {
                if (workTask.Description.Equals(workTaskDescription))
                    return false;
            }

            return true;
        }
    }
}