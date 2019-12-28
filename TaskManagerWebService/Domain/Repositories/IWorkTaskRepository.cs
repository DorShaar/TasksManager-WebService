using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerWebService.Domain.Models;

namespace TaskManagerWebService.Domain.Repositories
{
    public interface IWorkTaskRepository
    {
        Task<IEnumerable<WorkTask>> ListAsync();
    }
}