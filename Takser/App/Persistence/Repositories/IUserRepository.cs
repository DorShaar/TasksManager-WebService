using Takser.Domain.Models;

namespace Takser.App.Persistence.Repositories
{
    public interface IUserRepository
    {
        public User GetByUsernameAndPassword(string username, string password);
    }
}