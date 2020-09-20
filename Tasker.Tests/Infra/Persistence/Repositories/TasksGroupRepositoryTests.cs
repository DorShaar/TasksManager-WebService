using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using ObjectSerializer.JsonService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Takser.App.Persistence.Context;
using Takser.Infra.Options;
using TaskData;
using TaskData.IDsProducer;
using TaskData.TasksGroups;
using Tasker.Infra.Persistence.Context;
using Tasker.Infra.Persistence.Repositories;
using Xunit;

namespace Tasker.Tests.Infra.Persistence.Repositories
{
    public class TasksGroupRepositoryTests
    {
        private readonly ITasksGroupFactory mTasksGroupFactory;
        private readonly IObjectSerializer mObjectSerializer;
        private readonly IIDProducer mIDProducer;

        public TasksGroupRepositoryTests()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.UseTaskerDataEntities();
            serviceCollection.UseJsonObjectSerializer();
            ServiceProvider serviceProvider = serviceCollection
                .AddLogging()
                .BuildServiceProvider();

            mTasksGroupFactory = serviceProvider.GetRequiredService<ITasksGroupFactory>();
            mObjectSerializer = serviceProvider.GetRequiredService<IObjectSerializer>();
            mIDProducer = serviceProvider.GetRequiredService<IIDProducer>();
        }

        [Fact]
        public async Task AddAsync_TaskGroupNotExist_Success()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("group1");

            Assert.False((await tasksGroupRepository.ListAsync().ConfigureAwait(false)).Any());

            await tasksGroupRepository.AddAsync(tasksGroup).ConfigureAwait(false);

            Assert.NotNull(await tasksGroupRepository.FindAsync(tasksGroup.ID).ConfigureAwait(false));
            Assert.Single(await tasksGroupRepository.ListAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task AddAsync_TaskGroupAlreadyExist_NotAdded()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("group1");

            Assert.False((await tasksGroupRepository.ListAsync().ConfigureAwait(false)).Any());

            // Make tasksGroup be already exist.
            database.Entities.Add(tasksGroup);

            await tasksGroupRepository.AddAsync(tasksGroup).ConfigureAwait(false);

            Assert.NotNull(await tasksGroupRepository.FindAsync(tasksGroup.ID).ConfigureAwait(false));
            Assert.Single(await tasksGroupRepository.ListAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task AddAsync_TaskGroupNameAlreadyExist_NotAdded()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("group1");
            ITasksGroup tasksGroupWithSameName = mTasksGroupFactory.CreateGroup("group1");

            Assert.NotEqual(tasksGroup.ID, tasksGroupWithSameName.ID);

            Assert.False((await tasksGroupRepository.ListAsync().ConfigureAwait(false)).Any());

            await tasksGroupRepository.AddAsync(tasksGroup).ConfigureAwait(false);
            await tasksGroupRepository.AddAsync(tasksGroupWithSameName).ConfigureAwait(false);

            Assert.NotNull(await tasksGroupRepository.FindAsync(tasksGroup.ID).ConfigureAwait(false));
            Assert.Null(await tasksGroupRepository.FindAsync(tasksGroupWithSameName.ID).ConfigureAwait(false));
            Assert.Single(await tasksGroupRepository.ListAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task AddAsync_DatabaseIsLoadedAndSavedOnce()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            await tasksGroupRepository.AddAsync(A.Fake<ITasksGroup>()).ConfigureAwait(false);

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task FindAsync_IdExist_Found()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("group1");

            database.Entities.Add(tasksGroup);

            Assert.NotNull(await tasksGroupRepository.FindAsync(tasksGroup.ID).ConfigureAwait(false));
        }

        [Fact]
        public async Task FindAsync_IdNotExist_NotFound()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            Assert.Null(await tasksGroupRepository.FindAsync("1005").ConfigureAwait(false));
        }

        [Fact]
        public async Task FindAsync_DatabaseIsLoadedOnceAndNoteSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            await tasksGroupRepository.FindAsync("group1").ConfigureAwait(false);

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }

        [Fact]
        public async Task ListAsync_AsExpected()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            ITasksGroup tasksGroup1 = mTasksGroupFactory.CreateGroup("group1");
            ITasksGroup tasksGroup2 = mTasksGroupFactory.CreateGroup("group2");

            database.Entities.Add(tasksGroup1);
            database.Entities.Add(tasksGroup2);

            List<ITasksGroup> tasksGroups = (await tasksGroupRepository.ListAsync().ConfigureAwait(false)).ToList();
            Assert.Equal(tasksGroup1, tasksGroups[0]);
            Assert.Equal(tasksGroup2, tasksGroups[1]);
        }

        [Fact]
        public async Task ListAsync_HasNoGroups_ReturnsEmptyList()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            Assert.Empty(await tasksGroupRepository.ListAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task ListAsync_DatabaseIsLoadedOnceAndNoteSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            await tasksGroupRepository.ListAsync().ConfigureAwait(false);

            A.CallTo(() => database.LoadDatabase()).MustHaveHappenedOnceExactly();
            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }

        [Fact]
        public async Task RemoveAsync_GroupExists_GroupRemoved()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            ITasksGroup tasksGroup1 = mTasksGroupFactory.CreateGroup("group1");

            database.Entities.Add(tasksGroup1);

            Assert.Single(await tasksGroupRepository.ListAsync().ConfigureAwait(false));

            await tasksGroupRepository.RemoveAsync(tasksGroup1).ConfigureAwait(false);

            Assert.Empty(await tasksGroupRepository.ListAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task RemoveAsync_GroupNotExists_RemoveNotPerformed()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            ITasksGroup tasksGroup1 = mTasksGroupFactory.CreateGroup("group1");
            ITasksGroup tasksGroup2 = mTasksGroupFactory.CreateGroup("group2");

            database.Entities.Add(tasksGroup1);

            Assert.Single(await tasksGroupRepository.ListAsync().ConfigureAwait(false));

            await tasksGroupRepository.RemoveAsync(tasksGroup2).ConfigureAwait(false);

            Assert.Single(await tasksGroupRepository.ListAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task RemoveAsync_GroupExists_DatabaseIsLoadedAndSavedOnce()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("group1");
            database.Entities.Add(tasksGroup);

            await tasksGroupRepository.RemoveAsync(tasksGroup).ConfigureAwait(false);

            A.CallTo(() => database.LoadDatabase()).MustNotHaveHappened();
            A.CallTo(() => database.SaveCurrentDatabase()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateAsync_GroupExists_GroupUpdated()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("group1");

            database.Entities.Add(tasksGroup);

            Assert.Single(await tasksGroupRepository.ListAsync().ConfigureAwait(false));

            ITasksGroup tasksGroupWithUpdate = await tasksGroupRepository.FindAsync(tasksGroup.ID).ConfigureAwait(false);

            const string newGroupName = "group1_changed";

            tasksGroupWithUpdate.SetGroupName(newGroupName);

            await tasksGroupRepository.UpdateAsync(tasksGroupWithUpdate).ConfigureAwait(false);

            Assert.Single(await tasksGroupRepository.ListAsync().ConfigureAwait(false));

            ITasksGroup updatedTasksGroup = await tasksGroupRepository.FindAsync(tasksGroup.ID).ConfigureAwait(false);

            Assert.Equal(newGroupName, updatedTasksGroup.Name);
        }

        [Fact]
        public async Task UpdateAsync_GroupNotExists_GroupNotAdded()
        {
            AppDbContext database = new AppDbContext(
                    Options.Create(new DatabaseConfigurtaion()),
                    mObjectSerializer,
                    mIDProducer,
                    NullLogger<AppDbContext>.Instance);

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            ITasksGroup tasksGroup = mTasksGroupFactory.CreateGroup("group1");

            await tasksGroupRepository.UpdateAsync(tasksGroup).ConfigureAwait(false);

            Assert.Empty(await tasksGroupRepository.ListAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task UpdateAsync_GroupNotExists_DatabaseIsNotLoadedNorSaved()
        {
            IAppDbContext database = A.Fake<IAppDbContext>();

            TasksGroupRepository tasksGroupRepository =
                    new TasksGroupRepository(database, NullLogger<TasksGroupRepository>.Instance);

            await tasksGroupRepository.UpdateAsync(A.Fake<ITasksGroup>()).ConfigureAwait(false);

            A.CallTo(() => database.LoadDatabase()).MustNotHaveHappened();
            A.CallTo(() => database.SaveCurrentDatabase()).MustNotHaveHappened();
        }
    }
}