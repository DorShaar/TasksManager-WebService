using System.Collections.Generic;
using System.Threading.Tasks;
using Tasker.App.Persistence.Repositories;
using Tasker.App.Services;
using Tasker.Domain.Models;

namespace TaskManagerWebService.Services
{
    public class WorkTaskService : IWorkTaskService
    {
        private readonly IDbRepository<WorkTask> mWorkTaskRepository;

        public WorkTaskService(IDbRepository<WorkTask> workTaskRepository)
        {
            mWorkTaskRepository = workTaskRepository;
        }

        public async Task<IEnumerable<WorkTask>> ListAsync()
        {
            return await mWorkTaskRepository.ListAsync();
        }
    }
}