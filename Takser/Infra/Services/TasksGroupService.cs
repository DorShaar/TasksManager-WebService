using Logger.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takser.Infra.Options;
using TaskData.Contracts;
using Tasker.App.Persistence.Repositories;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Tasker.Domain.Validators;

namespace Tasker.Infra.Services
{
    public class TasksGroupService : ITasksGroupService
    {
        private readonly IDbRepository<ITasksGroup> mTasksGroupRepository;
        private readonly ITasksGroupBuilder mTaskGroupBuilder;
        private readonly NameValidator mNameValidator;
        private readonly ILogger mLogger;

        public TasksGroupService(IDbRepository<ITasksGroup> TaskGroupRepository, ITasksGroupBuilder tasksGroupBuilder, ILogger logger)
        {
            mTasksGroupRepository = TaskGroupRepository;
            mTaskGroupBuilder = tasksGroupBuilder;
            mLogger = logger;
            mNameValidator = new NameValidator(NameLengths.MaximalGroupNameLength);
        }

        public async Task<IEnumerable<ITasksGroup>> ListAsync()
        {
            return await mTasksGroupRepository.ListAsync();
        }

        public async Task<Response<ITasksGroup>> SaveAsync(string groupName)
        {
            try
            {
                ITasksGroup tasksGroup = mTaskGroupBuilder.Create(groupName, mLogger);
                await mTasksGroupRepository.AddAsync(tasksGroup);

                return new Response<ITasksGroup>(tasksGroup, isSuccess: true);
            }
            catch (Exception ex)
            {
                return new Response<ITasksGroup>(isSuccess: false, $"An error occurred when saving tasks grou name {groupName}: {ex.Message}");
            }
        }

        public async Task<Response<ITasksGroup>> UpdateAsync(string id, string newGroupName)
        {
            ITasksGroup groupToUpdate = 
                await mTasksGroupRepository.FindAsync(id);

            if (groupToUpdate == null)
                return new Response<ITasksGroup>(isSuccess: false, "Group not found");

            if (mNameValidator.IsNameValid(newGroupName))
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
    }
}