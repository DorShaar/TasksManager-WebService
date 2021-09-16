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
    }
}