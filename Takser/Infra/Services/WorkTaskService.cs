using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Takser.Domain.Communication;
using TaskData.Contracts;
using Tasker.App.Persistence.Repositories;
using Tasker.App.Services;

namespace Tasker.Infra.Services
{
    public class WorkTaskService : IWorkTaskService
    {
        private readonly IDbRepository<IWorkTask> mWorkTaskRepository;

        public WorkTaskService(IDbRepository<IWorkTask> workTaskRepository)
        {
            mWorkTaskRepository = workTaskRepository;
        }

        public async Task<IEnumerable<IWorkTask>> ListAsync()
        {
            return await mWorkTaskRepository.ListAsync();
        }

        public async Task<WorkTaskResponse> SaveAsync(IWorkTask workTask)
        {
            try
            {
                await mWorkTaskRepository.AddAsync(workTask);

                return new WorkTaskResponse(workTask);
            }
            catch (Exception ex)
            {
                return new WorkTaskResponse($"An error occurred when saving work task id {workTask.ID}: {ex.Message}");
            }
        }

        public async Task<WorkTaskResponse> UpdateAsync(string id, IWorkTask newWorkTask)
        {
            if (id != newWorkTask.ID)
                return new WorkTaskResponse($"Task to update with id {newWorkTask.ID} not match id {id}");

            IWorkTask workTaskToUpdate = await mWorkTaskRepository.FindByIdAsync(id);

            if (workTaskToUpdate == null)
                return new WorkTaskResponse("Work task not found");

            workTaskToUpdate = newWorkTask;

            try
            {
                await mWorkTaskRepository.UpdateAsync(workTaskToUpdate);

                return new WorkTaskResponse(workTaskToUpdate);
            }
            catch (Exception ex)
            {
                return new WorkTaskResponse($"An error occurred when updating work task id {id}: {ex.Message}");
            }
        }

        public async Task<WorkTaskResponse> RemoveAsync(string id)
        {
            IWorkTask taskToRemove = await mWorkTaskRepository.FindByIdAsync(id);
            if (taskToRemove == null)
                return new WorkTaskResponse(taskToRemove, $"Work task {id} not found. No deletion performed");

            try
            {
                await mWorkTaskRepository.RemoveAsync(taskToRemove);

                return new WorkTaskResponse(taskToRemove);
            }
            catch (Exception ex)
            {
                return new WorkTaskResponse($"An error occurred when removing work task id {id}: {ex.Message}");
            }
        }
    }
}