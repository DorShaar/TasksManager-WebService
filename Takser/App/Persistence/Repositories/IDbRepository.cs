using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tasker.App.Persistence.Repositories
{
    public interface IDbRepository<T>
    {
        Task<IEnumerable<T>> ListAsync();
        Task AddAsync(T entity);
        Task<T> FindByIdAsync(string id);
        Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
    }
}