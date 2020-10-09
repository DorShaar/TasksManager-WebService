using System.Threading.Tasks;

namespace Tasker.App.Services
{
    public interface INotifierService
    {
        Task NotifyTriangleTasks();
        Task NotifySummary();
    }
}