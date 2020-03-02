using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tasker.App.Persistence.Repositories;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Tasker.Domain.Models;

namespace TaskManagerWebService.Services
{
    public class TasksGroupService : ITasksGroupService
    {
        private readonly IDbRepository<TasksGroup> mTasksGroupRepository;

        public TasksGroupService(IDbRepository<TasksGroup> workTaskRepository)
        {
            mTasksGroupRepository = workTaskRepository;
        }

        public async Task<IEnumerable<TasksGroup>> ListAsync()
        {
            return await mTasksGroupRepository.ListAsync();
        }

        public async Task<TasksGroupResponse> SaveAsync(TasksGroup group)
        {
            try
            {
                await mTasksGroupRepository.AddAsync(group);

                return new TasksGroupResponse(group);
            }
            catch (Exception ex)
            {
                return new TasksGroupResponse($"An error occurred when saving the category: {ex.Message}");
            }
        }

        public async Task<TasksGroupResponse> UpdateAsync(string id, TasksGroup newGroup)
        {
            TasksGroup groupToUpdate = 
                await mTasksGroupRepository.FindByIdAsync(id);

            if (groupToUpdate == null)
                return new TasksGroupResponse("Group not found");

            groupToUpdate.Name = newGroup.Name;

            try
            {
                await mTasksGroupRepository.UpdateAsync(groupToUpdate);

                return new TasksGroupResponse(groupToUpdate);
            }
            catch (Exception ex)
            {
                return new TasksGroupResponse($"An error occurred when updating the category: {ex.Message}");
            }
        }

        public async Task<TasksGroupResponse> RemoveAsync(string id)
        {
            TasksGroup groupToRemove = await mTasksGroupRepository.FindByIdAsync(id);
            if (groupToRemove == null)
                return new TasksGroupResponse(groupToRemove, $"Entity group {id} not found. No deletion performed");

            try
            {
                await mTasksGroupRepository.RemoveAsync(groupToRemove);

                return new TasksGroupResponse(groupToRemove);
            }
            catch (Exception ex)
            {
                return new TasksGroupResponse($"An error occurred when updating the category: {ex.Message}");
            }
        }
    }
}