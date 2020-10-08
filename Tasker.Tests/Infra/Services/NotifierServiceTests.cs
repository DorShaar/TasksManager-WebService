using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
using Tasker.App.Services;
using Tasker.Infra.Services.Notifier;
using Triangle;
using Triangle.Time;
using Xunit;

namespace Tasker.Tests.Infra.Services
{
    public class NotifierServiceTests
    {
        [Fact]
        public async Task Notify_HasTasksToNotify_Notify()
        {
            IEmailService emailService = A.Fake<IEmailService>();
            ITasksGroupService tasksGroupService  = A.Fake<ITasksGroupService>();

            TaskTriangleBuilder taskTriangleBuilder = new TaskTriangleBuilder();
            TaskTriangle taskTriangle = taskTriangleBuilder.AddContent("content1")
                               .AddResource("resource1")
                               .AddPercentageProgressToNotify(60)
                               .SetTime("20/09/2020", DayPeriod.Noon, 4, halfWorkDay: true)
                               .Build();

            IWorkTask workTask = A.Fake<IWorkTask>();
            A.CallTo(() => workTask.TaskMeasurement).Returns(taskTriangle);

            List<IWorkTask> workTasks = new List<IWorkTask>()
            {
                workTask 
            };

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            A.CallTo(() => tasksGroup.GetAllTasks()).Returns(workTasks);

            List<ITasksGroup> tasksGroups = new List<ITasksGroup>()
            {
                tasksGroup
            };
            A.CallTo(() => tasksGroupService.ListAsync()).Returns(tasksGroups);

            NotifierService notifierService = new NotifierService(
                emailService, tasksGroupService, NullLogger<NotifierService>.Instance);

            await notifierService.Notify().ConfigureAwait(false);

            A.CallTo(() => emailService.SendEmail(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Notify_HasNoTasksToNotify_DoesNotNotify()
        {
            IEmailService emailService = A.Fake<IEmailService>();
            ITasksGroupService tasksGroupService = A.Fake<ITasksGroupService>();

            TaskTriangleBuilder taskTriangleBuilder = new TaskTriangleBuilder();
            TaskTriangle taskTriangle = taskTriangleBuilder.AddContent("content1")
                               .AddResource("resource1")
                               .AddPercentageProgressToNotify(60)
                               .SetTime(DateTime.Now.ToString(TimeConsts.TimeFormat), 
                                        DayPeriod.Noon, 4, halfWorkDay: true)
                               .Build();

            IWorkTask workTask = A.Fake<IWorkTask>();
            A.CallTo(() => workTask.TaskMeasurement).Returns(taskTriangle);

            List<IWorkTask> workTasks = new List<IWorkTask>()
            {
                workTask
            };

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            A.CallTo(() => tasksGroup.GetAllTasks()).Returns(workTasks);

            List<ITasksGroup> tasksGroups = new List<ITasksGroup>()
            {
                tasksGroup
            };
            A.CallTo(() => tasksGroupService.ListAsync()).Returns(tasksGroups);

            NotifierService notifierService = new NotifierService(
                emailService, tasksGroupService, NullLogger<NotifierService>.Instance);

            await notifierService.Notify().ConfigureAwait(false);

            A.CallTo(() => emailService.SendEmail(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Notify_HasTasksToNotify_NotifyOnlyOnce()
        {
            IEmailService emailService = A.Fake<IEmailService>();
            ITasksGroupService tasksGroupService = A.Fake<ITasksGroupService>();

            TaskTriangleBuilder taskTriangleBuilder = new TaskTriangleBuilder();
            TaskTriangle taskTriangle = taskTriangleBuilder.AddContent("content1")
                .AddResource("resource1")
                .AddPercentageProgressToNotify(10)
                .SetTime(DateTime.Now.ToString(TimeConsts.TimeFormat), DayPeriod.Morning, 4, halfWorkDay: true)
                .Build();

            IWorkTask workTask = A.Fake<IWorkTask>();
            A.CallTo(() => workTask.TaskMeasurement).Returns(taskTriangle);

            List<IWorkTask> workTasks = new List<IWorkTask>()
            {
                workTask
            };

            ITasksGroup tasksGroup = A.Fake<ITasksGroup>();
            A.CallTo(() => tasksGroup.GetAllTasks()).Returns(workTasks);

            List<ITasksGroup> tasksGroups = new List<ITasksGroup>()
            {
                tasksGroup
            };
            A.CallTo(() => tasksGroupService.ListAsync()).Returns(tasksGroups);

            NotifierService notifierService = new NotifierService(
                emailService, tasksGroupService, NullLogger<NotifierService>.Instance);

            await notifierService.Notify().ConfigureAwait(false);
            await notifierService.Notify().ConfigureAwait(false);

            A.CallTo(() => emailService.SendEmail(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}