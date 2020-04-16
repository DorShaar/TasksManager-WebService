using System.Linq;
using Takser.App.Persistence.Context;
using Takser.App.Persistence.Repositories;
using Takser.Domain.Models;
using Takser.Infra.Extensions;

namespace Takser.Infra.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IUsersDbContext mUsersDatabase;

        public UserRepository(IUsersDbContext userDatabase)
        {
            mUsersDatabase = userDatabase;
        }

        public User GetByUsernameAndPassword(string username, string password)
        {
            User user = mUsersDatabase.Users.SingleOrDefault(u =>
                u.Name.Equals(username) && u.HashedPassword.Equals(password.ToSha256()));

            return user;
        }
    }
}