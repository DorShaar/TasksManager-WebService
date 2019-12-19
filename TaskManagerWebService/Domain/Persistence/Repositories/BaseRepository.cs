using MyFirstWebApp.Domain.Persistence.Context;

namespace MyFirstWebApp.Domain.Persistence.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly AppDbContext mContext;

        public BaseRepository(AppDbContext context)
        {
            mContext = context;
        }
    }
}