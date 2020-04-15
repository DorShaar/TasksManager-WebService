using Logger.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskData.Contracts;
using Tasker.App.Persistence.Repositories;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Tasker.Domain.Options;
using Tasker.Domain.Validators;

namespace Tasker.Infra.Services
{
    public class WorkTaskService : IWorkTaskService
    {
        private readonly IDbRepository<IWorkTask> mWorkTaskRepository;
        private readonly NameValidator mNameValidator;
        private readonly ILogger mLogger;

        public WorkTaskService(IDbRepository<IWorkTask> workTaskRepository, ILogger logger)
        {
            mWorkTaskRepository = workTaskRepository;
            mLogger = logger;
            mNameValidator = new NameValidator(NameLengths.MaximalTaskNameLength);
        }

        public async Task<IEnumerable<IWorkTask>> ListAsync()
        {
            return await mWorkTaskRepository.ListAsync();
        }

        public async Task<Response<IWorkTask>> SaveAsync(IWorkTask workTask)
        {
            //ITasksGroup taskGroup = mTaskManager.GetAllTasksGroups().Where(group => group.ID == taskGroupName).FirstOrDefault();
            //if (taskGroup == null)
            //    taskGroup = mTaskManager.GetAllTasksGroups().Where(group => group.Name == taskGroupName).FirstOrDefault();

            //if (taskGroup == null)
            //{
            //    mLogger.LogError($"Task group {taskGroupName} does not exist");
            //    return 1;
            //}

            //if (tasksGroup == null)
            //{
            //    mLogger.LogError($"Given task group is null");
            //    return null;
            //}

            //IWorkTask task = tasksGroup.CreateTask(description);
            //mTasksDatabase.AddOrUpdate(tasksGroup);
            //return task;

            try
            {
                await mWorkTaskRepository.AddAsync(workTask);

                return new Response<IWorkTask>(workTask, isSuccess: true);
            }
            catch (Exception ex)
            {
                return new Response<IWorkTask>(isSuccess: false, $"An error occurred when saving work task id {workTask.ID}: {ex.Message}");
            }
        }

        public async Task<Response<IWorkTask>> UpdateAsync(string id, IWorkTask newWorkTask)
        {
            if (id != newWorkTask.ID)
                return new Response<IWorkTask>(isSuccess: false, $"Task to update with id {newWorkTask.ID} not match id {id}");

            IWorkTask workTaskToUpdate = await mWorkTaskRepository.FindAsync(id);

            if (workTaskToUpdate == null)
                return new Response<IWorkTask>(isSuccess: false, "Work task not found");

            workTaskToUpdate = newWorkTask;

            try
            {
                await mWorkTaskRepository.UpdateAsync(workTaskToUpdate);

                return new Response<IWorkTask>(workTaskToUpdate, isSuccess: true);
            }
            catch (Exception ex)
            {
                return new Response<IWorkTask>(isSuccess: false, $"An error occurred when updating work task id {id}: {ex.Message}");
            }
        }

        public async Task<Response<IWorkTask>> RemoveAsync(string id)
        {
            IWorkTask taskToRemove = await mWorkTaskRepository.FindAsync(id);
            if (taskToRemove == null)
                return new Response<IWorkTask>(isSuccess: false, $"Work task {id} not found. No deletion performed");

            try
            {
                await mWorkTaskRepository.RemoveAsync(taskToRemove);

                return new Response<IWorkTask>(taskToRemove, isSuccess: true);
            }
            catch (Exception ex)
            {
                return new Response<IWorkTask>(isSuccess: false, $"An error occurred when removing work task id {id}: {ex.Message}");
            }
        }
    }
}

//public Task AddAsync(ITasksGroup tasksGroup, string taskDescription)
//{
//    tasksGroup.CreateTask(taskDescription);
//    mDatabase.SaveCurrentDatabase();

//    return Task.CompletedTask;
//}

//public async Task<IEnumerable<IWorkTask>> FindWorkTasksAsyncByWorkTaskCondition(Func<IWorkTask, bool> condition)
//{
//    mDatabase.LoadDatabase();

//    List<IWorkTask> workTasks = new List<IWorkTask>();

//    foreach (IWorkTask task in await ListAsync())
//    {
//        if (condition(task))
//            workTasks.Add(task);
//    }

//    mLogger.Log($"Found {workTasks.Count} tasks");
//    return workTasks;
//}

//public Task<IEnumerable<IWorkTask>> FindWorkTasksAsyncByTasksGroupCondition(Func<ITasksGroup, bool> condition)
//{
//    mDatabase.LoadDatabase();

//    List<IWorkTask> workTasks = new List<IWorkTask>();

//    foreach (ITasksGroup taskGroup in mDatabase.Entities)
//    {
//        if (condition(taskGroup))
//            workTasks.AddRange(taskGroup.GetAllTasks());
//    }

//    mLogger.Log($"Found {workTasks.Count} tasks");
//    return Task.FromResult(workTasks.AsEnumerable());
//}

//public Task<IEnumerable<IWorkTask>> ListAsync()
//{
//    mDatabase.LoadDatabase();

//    List<IWorkTask> allTasks = new List<IWorkTask>();

//    foreach (ITasksGroup taskGroup in mDatabase.Entities)
//    {
//        allTasks.AddRange(taskGroup.GetAllTasks());
//    }

//    return Task.FromResult(allTasks.AsEnumerable());
//}

//public async Task RemoveAsync(string workTaskId)
//{
//    mDatabase.LoadDatabase();

//    foreach (ITasksGroup group in await ListAsync())
//    {
//        IWorkTask task = group.GetTask(workTaskId);
//        if (task != null)
//        {
//            group.RemoveTask(workTaskId);
//            mDatabase.SaveCurrentDatabase();
//            return;
//        }
//    }

//    mLogger.LogError($"Task {workTaskId} was not found");
//}

//public Task UpdateAsync(IWorkTask workTask)
//{
//    throw new System.NotImplementedException();
//}