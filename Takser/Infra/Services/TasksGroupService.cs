using System;
using System.Collections.Generic;
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
        private readonly NameValidator mNameValidator;

        public TasksGroupService(IDbRepository<ITasksGroup> TaskGroupRepository)
        {
            mTasksGroupRepository = TaskGroupRepository;
            mNameValidator = new NameValidator(NameLengths.MaximalGroupNameLength);
        }

        public async Task<IEnumerable<ITasksGroup>> ListAsync()
        {
            return await mTasksGroupRepository.ListAsync();
        }

        public async Task<TasksGroupResponse> SaveAsync(ITasksGroup group)
        {
            try
            {
                await mTasksGroupRepository.AddAsync(group);

                return new TasksGroupResponse(group);
            }
            catch (Exception ex)
            {
                return new TasksGroupResponse($"An error occurred when saving tasks groups id {group.ID}: {ex.Message}");
            }
        }

        public async Task<TasksGroupResponse> UpdateAsync(string id, string newGroupName)
        {
            ITasksGroup groupToUpdate = 
                await mTasksGroupRepository.FindByIdAsync(id);

            if (groupToUpdate == null)
                return new TasksGroupResponse("Group not found");

            if (mNameValidator.IsNameValid(newGroupName))
                return new TasksGroupResponse($"Group name '{newGroupName}' is invalid");

            groupToUpdate.Name = newGroupName;

            try
            {
                await mTasksGroupRepository.UpdateAsync(groupToUpdate);

                return new TasksGroupResponse(groupToUpdate);
            }
            catch (Exception ex)
            {
                return new TasksGroupResponse($"An error occurred when updating tasks group id {id}: {ex.Message}");
            }
        }

        public async Task<TasksGroupResponse> RemoveAsync(string id)
        {
            ITasksGroup groupToRemove = await mTasksGroupRepository.FindByIdAsync(id);
            if (groupToRemove == null)
                return new TasksGroupResponse(groupToRemove, $"Entity group {id} not found. No deletion performed");

            try
            {
                await mTasksGroupRepository.RemoveAsync(groupToRemove);

                return new TasksGroupResponse(groupToRemove);
            }
            catch (Exception ex)
            {
                return new TasksGroupResponse($"An error occurred when removing tasks group id {id}: {ex.Message}");
            }
        }
    }
}