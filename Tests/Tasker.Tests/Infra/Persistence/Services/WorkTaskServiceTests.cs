namespace Tasker.Tests.Infra.Persistence.Services
{
    public class WorkTaskServiceTests
    {
        //[Fact]
        //public async Task AddAsync_ParentGroupNotExist_NotAdded()
        //{
        //    AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

        //    WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

        //    ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());

        //    database.Entities.Add(tasksGroup);

        //    //WorkTask workTask = new WorkTask("group2", "worktask1", A.Fake<ILogger>());

        //    Assert.Empty(await workTaskRepository.ListAsync());

        //    await workTaskRepository.AddAsync(workTask);

        //    Assert.Null(await workTaskRepository.FindAsync(workTask.ID));
        //    Assert.Empty(await workTaskRepository.ListAsync());
        //}

        //[Fact]
        //public async Task RemoveAsync_TaskExists_TaskRemoved()
        //{
        //    AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

        //    WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

        //    ITasksGroup tasksGroup1 = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
        //    IWorkTask workTask = tasksGroup1.CreateTask("taskDescription");

        //    await database.SaveCurrentDatabase();

        //    Assert.Single(await workTaskRepository.ListAsync());

        //    await workTaskRepository.RemoveAsync(workTask);

        //    Assert.Empty(await workTaskRepository.ListAsync());
        //}

        //[Fact]
        //public async Task RemoveAsync_WorkTaskNotExists_RemoveNotPerformed()
        //{
        //    AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

        //    WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

        //    string taskDescription = "taskDescripition";

        //    ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
        //    tasksGroup.CreateTask(taskDescription);

        //    database.Entities.Add(tasksGroup);

        //    Assert.Single(await workTaskRepository.ListAsync());

        //    //WorkTask differentWorkTaskWithSameGroupNameAndDescription = new WorkTask("group1", taskDescription, A.Fake<ILogger>());

        //    await workTaskRepository.RemoveAsync(differentWorkTaskWithSameGroupNameAndDescription);

        //    Assert.Single(await workTaskRepository.ListAsync());
        //}

        //[Fact]
        //public async Task RemoveAsync_DatabaseNotLoadedButSavedOnce()
        //{
        //    IAppDbContext database = A.Fake<IAppDbContext>();

        //    WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

        //    await workTaskRepository.RemoveAsync(A.Fake<IWorkTask>());

        //    A.CallTo(() => database.LoadDatabase()).MustNotHaveHappened();
        //    A.CallTo(() => database.SaveCurrentDatabase()).MustHaveHappenedOnceExactly();
        //}

        //[Fact]
        //public async Task AddAsync_DescriptionExistsInTheSameGroup_NotAdded()
        //{
        //    string tempDirectory = CopyDirectoryToTempDirectory(mNewDatabaseDirectoryPath);

        //    try
        //    {
        //        DatabaseConfigurtaion databaseConfigurtaion = new DatabaseConfigurtaion
        //        {
        //            DatabaseDirectoryPath = tempDirectory
        //        };

        //        AppDbContext database = new AppDbContext(Options.Create(databaseConfigurtaion), new JsonSerializerWrapper(), A.Fake<ILogger>());

        //        WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

        //        string tasksGroupName = "group1";
        //        string taskDescription = "workTask1";

        //        ITasksGroup tasksGroup = mTasksGroupBuilder.Create(tasksGroupName, A.Fake<ILogger>());
        //        tasksGroup.CreateTask(taskDescription);

        //        database.Entities.Add(tasksGroup);
        //        await database.SaveCurrentDatabase();

        //        Assert.Single(await workTaskRepository.ListAsync());

        //        //WorkTask workTask = new WorkTask(tasksGroupName, taskDescription, A.Fake<ILogger>());
        //        await workTaskRepository.AddAsync(workTask);

        //        Assert.NotNull(await workTaskRepository.FindAsync(workTask.ID));
        //        Assert.Single(await workTaskRepository.ListAsync());
        //    }
        //    finally
        //    {
        //        Directory.Delete(tempDirectory, recursive: true);
        //    }
        //}

        //[Fact]
        //public async Task UpdateAsync_WorkTaskExists_WorkTaskUpdated()
        //{
        //    AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

        //    WorkTaskRepository workTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

        //    ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
        //    IWorkTask workTask = tasksGroup.CreateTask("taskDescription");

        //    database.Entities.Add(tasksGroup);

        //    Assert.Single(await workTaskRepository.ListAsync());

        //    IWorkTask workTaskWithUpdate = await workTaskRepository.FindAsync(workTask.ID);

        //    string newTaskSecription = "description_changed";

        //    workTaskWithUpdate.Description = newTaskSecription;

        //    await workTaskRepository.UpdateAsync(workTaskWithUpdate);

        //    Assert.Single(await workTaskRepository.ListAsync());

        //    IWorkTask updatedTasksGroup = await workTaskRepository.FindAsync(workTaskWithUpdate.ID);

        //    Assert.Equal(newTaskSecription, updatedTasksGroup.Description);
        //}

        //[Fact]
        //public async Task UpdateAsync_WorkTaskNotExists_WorkTaskNotAdded()
        //{
        //    AppDbContext database = new AppDbContext(Options.Create(new DatabaseConfigurtaion()), A.Fake<IObjectSerializer>(), A.Fake<ILogger>());

        //    WorkTaskRepository WorkTaskRepository = new WorkTaskRepository(database, A.Fake<ILogger>());

        //    ITasksGroup tasksGroup = mTasksGroupBuilder.Create("group1", A.Fake<ILogger>());
        //    tasksGroup.CreateTask("description1");

        //    database.Entities.Add(tasksGroup);

        //    Assert.Single(await WorkTaskRepository.ListAsync());

        //    //WorkTask workTask = new WorkTask("group1", "description2", A.Fake<ILogger>());

        //    await WorkTaskRepository.UpdateAsync(workTask);

        //    Assert.Single(await WorkTaskRepository.ListAsync());
        //}
    }
}