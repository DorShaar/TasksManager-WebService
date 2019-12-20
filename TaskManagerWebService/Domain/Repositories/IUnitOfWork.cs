using System.Threading.Tasks;

namespace TaskManagerWebService.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}