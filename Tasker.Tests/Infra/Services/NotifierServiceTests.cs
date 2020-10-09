using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task NotifyTriangleTasks_HasTasksToNotify_Notify()
        {
            IEmailService emailService = A.Fake<IEmailService>();
            IWorkTaskService workTaskService  = A.Fake<IWorkTaskService>();

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

            A.CallTo(() => workTaskService.FindWorkTasksByConditionAsync(A<Func<IWorkTask, bool>>.Ignored))
                .Returns(workTasks);

            NotifierService notifierService = new NotifierService(
                emailService, workTaskService, NullLogger<NotifierService>.Instance);

            await notifierService.NotifyTriangleTasks().ConfigureAwait(false);

            A.CallTo(() => emailService.SendEmail(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task NotifyTriangleTasks_HasNoTasksToNotify_DoesNotNotify()
        {
            IEmailService emailService = A.Fake<IEmailService>();
            IWorkTaskService workTaskService = A.Fake<IWorkTaskService>();

            A.CallTo(() => workTaskService.FindWorkTasksByConditionAsync(A<Func<IWorkTask, bool>>.Ignored))
                .Returns(new List<IWorkTask>());

            NotifierService notifierService = new NotifierService(
                emailService, workTaskService, NullLogger<NotifierService>.Instance);

            await notifierService.NotifyTriangleTasks().ConfigureAwait(false);

            A.CallTo(() => emailService.SendEmail(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task NotifyTriangleTasks_HasTasksToNotify_NotifyOnlyOnce()
        {
            IEmailService emailService = A.Fake<IEmailService>();
            IWorkTaskService workTaskService = A.Fake<IWorkTaskService>();

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

            A.CallTo(() => workTaskService.FindWorkTasksByConditionAsync(A<Func<IWorkTask, bool>>.Ignored))
                .Returns(new List<IWorkTask>()).Once()
                .Then.Returns(workTasks).Once()
                .Then.Returns(new List<IWorkTask>()).Once()
                .Then.Returns(workTasks).Once();

            NotifierService notifierService = new NotifierService(
                emailService, workTaskService, NullLogger<NotifierService>.Instance);

            await notifierService.NotifyTriangleTasks().ConfigureAwait(false);
            await notifierService.NotifyTriangleTasks().ConfigureAwait(false);

            A.CallTo(() => emailService.SendEmail(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}