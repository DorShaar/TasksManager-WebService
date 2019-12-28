using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerWebService.Domain.Models;
using TaskManagerWebService.Domain.Repositories;
using TaskManagerWebService.Domain.Services;

namespace TaskManagerWebService.Services
{
    public class WorkTaskService : IWorkTaskService
    {
        private readonly IWorkTaskRepository mWorkTaskRepository;

        public WorkTaskService(IWorkTaskRepository workTaskRepository)
        {
            mWorkTaskRepository = workTaskRepository;
        }

        public async Task<IEnumerable<WorkTask>> ListAsync()
        {
            return await mWorkTaskRepository.ListAsync();
        }
    }
}