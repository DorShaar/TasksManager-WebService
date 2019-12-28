using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerWebService.Domain.Models;

namespace TaskManagerWebService.Domain.Services
{
    public interface IWorkTaskService
    {
        Task<IEnumerable<WorkTask>> ListAsync(); 
    }
}