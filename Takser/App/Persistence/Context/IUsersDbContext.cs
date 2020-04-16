using System.Collections.Generic;
using System.Threading.Tasks;
using Takser.Domain.Models;

namespace Takser.App.Persistence.Context
{
    public interface IUsersDbContext
    {
        HashSet<User> Users { get; }
        Task SaveDatabase();
    }
}