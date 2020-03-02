using Tasker.Infra.Persistence.Context;

namespace Tasker.Infra.Persistence.Repositories
{
    public class BaseRepository
    {
        public AppDbContext Context { get; }

        public BaseRepository(AppDbContext context)
        {
            Context = context;
        }
    }
}