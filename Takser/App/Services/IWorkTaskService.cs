using System.Collections.Generic;
using System.Threading.Tasks;
using Tasker.Domain.Models;

namespace Tasker.App.Services
{
    public interface IWorkTaskService
    {
        Task<IEnumerable<WorkTask>> ListAsync();
    }
}