using System.Collections.Generic;
using System.Threading.Tasks;
using Takser.App.Persistence.Context;
using Takser.Domain.Models;

namespace Takser.Infra.Persistence.Context
{
    public class UsersDbContext : IUsersDbContext
    {
        public HashSet<User> Users { get; } = new HashSet<User>();

        public UsersDbContext()
        {
            LoadDatabase();
        }

        private void LoadDatabase()
        {

        }

        public Task SaveDatabase()
        {
            return Task.CompletedTask;
        }
    }
}