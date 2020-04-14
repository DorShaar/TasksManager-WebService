using System.Threading.Tasks;
using Xunit;

namespace Tasker.Tests.Infra.Persistence.Services
{
    public class TasksGroupServiceTests
    {
        [Fact]
        public async Task RemoveAsync_GroupExist_GroupRemovedAndWorkTasksMoveToFreeGroup()
        {
            //AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            //TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database);

            //ITasksGroup tasksGroup1 = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());

            //database.Entities.Add(tasksGroup1);

            //Assert.Single(await tasksGroupRepository.ListAsync());

            //await tasksGroupRepository.RemoveAsync(tasksGroup1);

            //Assert.Empty(await tasksGroupRepository.ListAsync());
        }

        [Fact]
        public async Task AddAsync_InvalidGroupName_Redfsdfdsfsdfdsfsdf()
        {
            //AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

            //TasksGroupRepository tasksGroupRepository = new TasksGroupRepository(database);

            //ITasksGroup tasksGroup1 = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());

            //database.Entities.Add(tasksGroup1);

            //Assert.Single(await tasksGroupRepository.ListAsync());

            //await tasksGroupRepository.RemoveAsync(tasksGroup1);

            //Assert.Empty(await tasksGroupRepository.ListAsync());
        }
    }
}