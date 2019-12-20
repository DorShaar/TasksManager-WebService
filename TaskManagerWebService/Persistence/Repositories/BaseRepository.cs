using TaskManagerWebService.Persistence.Context;

namespace TaskManagerWebService.Persistence.Repositories
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