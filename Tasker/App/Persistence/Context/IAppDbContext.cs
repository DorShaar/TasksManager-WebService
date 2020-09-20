using System.Collections.Generic;
using System.Threading.Tasks;
using TaskData.TasksGroups;

namespace Takser.App.Persistence.Context
{
    public interface IAppDbContext
    {
        List<ITasksGroup> Entities { get; }

        Task LoadDatabase();
        Task SaveCurrentDatabase();
    }
}