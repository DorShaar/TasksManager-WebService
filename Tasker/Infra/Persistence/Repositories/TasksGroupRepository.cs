using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Takser.App.Persistence.Context;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
using Tasker.App.Persistence.Repositories;

namespace Tasker.Infra.Persistence.Repositories
{
    public class TasksGroupRepository : IDbRepository<ITasksGroup>
    {
        private readonly ILogger<TasksGroupRepository> mLogger;
        private readonly IAppDbContext mDatabase;

        public TasksGroupRepository(IAppDbContext database, ILogger<TasksGroupRepository> logger)
        {
            mDatabase = database ?? throw new ArgumentNullException(nameof(database));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task AddAsync(ITasksGroup newGroup)
        {
            mDatabase.LoadDatabase();

            if (mDatabase.Entities.Contains(newGroup) ||
               (mDatabase.Entities.Find(entity => entity.ID == newGroup.ID) != null))
            {
                mLogger.LogError($"Group ID: {newGroup.ID} is already found in database");
                return Task.CompletedTask;
            }

            if (mDatabase.Entities.Find(entity => entity.Name == newGroup.Name) != null)
            {
                mLogger.LogError($"Group name: {newGroup.Name} is already found in database");
                return Task.CompletedTask;
            }

            mDatabase.Entities.Add(newGroup);

            mDatabase.SaveCurrentDatabase();
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ITasksGroup>> ListAsync()
        {
            mDatabase.LoadDatabase();

            return Task.FromResult(mDatabase.Entities.AsEnumerable());
        }

        public Task<ITasksGroup> FindAsync(string entityToFind)
        {
            mDatabase.LoadDatabase();

            ITasksGroup entityFound =
                mDatabase.Entities.Find(entity => entity.ID == entityToFind) ??
                mDatabase.Entities.Find(entity => entity.Name == entityToFind);

            return Task.FromResult(entityFound);
        }

        public Task UpdateAsync(ITasksGroup newGroup)
        {
            int tasksGroupToUpdateIndex = mDatabase.Entities.FindIndex(entity => entity.ID == newGroup.ID);

            if (tasksGroupToUpdateIndex < 0)
            {
                mLogger.LogError($"Group ID: {newGroup.ID} Group name: {newGroup.Name} - No such entity was found in database");
                return Task.CompletedTask;
            }

            mDatabase.Entities[tasksGroupToUpdateIndex] = newGroup;

            mDatabase.SaveCurrentDatabase();
            return Task.CompletedTask;
        }

        public Task RemoveAsync(ITasksGroup group)
        {
            if (!mDatabase.Entities.Contains(group))
            {
                mLogger.LogError($"Group ID: {group.ID} Group name: {group.Name} - No such entity was found in database");
                return Task.CompletedTask;
            }

            foreach (IWorkTask task in group.GetAllTasks())
            {
                mLogger.LogDebug($"Removing inner task id {task.ID} description {task.Description}");
            }

            mDatabase.Entities.Remove(group);
            mLogger.LogDebug($"Task group id {group.ID} group name {group.Name} removed");

            mDatabase.SaveCurrentDatabase();
            return Task.CompletedTask;
        }
    }
}