using System.Threading.Tasks;
using TaskManagerWebService.Domain.Repositories;
using TaskManagerWebService.Persistence.Context;

namespace TaskManagerWebService.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext mContext;

        public UnitOfWork(AppDbContext context)
        {
            mContext = context;
        }

        public async Task CompleteAsync()
        {
            await mContext.SaveChangesAsync();
        }
    }
}