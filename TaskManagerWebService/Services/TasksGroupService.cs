using TaskManagerWebService.Domain.Models;
using TaskManagerWebService.Domain.Repositories;
using TaskManagerWebService.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerWebService.Domain.Services.Communication;
using System;

namespace TaskManagerWebService.Services
{
    public class TasksGroupService : ITasksGroupService
    {
        private readonly ITasksGroupRepository mTasksGroupRepository;
        private readonly IUnitOfWork mUnitOfWork;

        public TasksGroupService(ITasksGroupRepository workTaskRepository, IUnitOfWork unitOfWork)
        {
            mTasksGroupRepository = workTaskRepository;
            mUnitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TasksGroup>> ListAsync()
        {
            return await mTasksGroupRepository.ListAsync();
        }

        public async Task<SaveTasksGroupResponse> SaveAsync(TasksGroup group)
        {
            try
            {
                await mTasksGroupRepository.AddAsync(group);
                await mUnitOfWork.CompleteAsync();

                return new SaveTasksGroupResponse(group);
            }
            catch (Exception ex)
            {
                return new SaveTasksGroupResponse($"An error occurred when saving the category: {ex.Message}");
            }
        }

        public async Task<SaveTasksGroupResponse> UpdateAsync(string id, TasksGroup newGroup)
        {
            TasksGroup groupToUpdate = await mTasksGroupRepository.FindByIdAsync(id);

            if (groupToUpdate == null)
                return new SaveTasksGroupResponse("Category not found");

            groupToUpdate.GroupName = newGroup.GroupName;

            try
            {
                mTasksGroupRepository.Update(groupToUpdate);
                await mUnitOfWork.CompleteAsync();

                return new SaveTasksGroupResponse(groupToUpdate);
            }
            catch (Exception ex)
            {
                return new SaveTasksGroupResponse($"An error occurred when updating the category: {ex.Message}");
            }
        }
    }
}