using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaskData.WorkTasks;
using Tasker.App.Services;
using Tasker.Domain.Models;

namespace Tasker.Infra.Services.Notifier
{
    public class NotifierService : INotifierService
    {
        private readonly ILogger<NotifierService> mLogger;
        private readonly IEmailService mEmailService;
        private readonly IWorkTaskService mWorkTaskService;

        public NotifierService(IEmailService emailService,
            IWorkTaskService workTaskService,
            ILogger<NotifierService> logger)
        {
            mEmailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            mWorkTaskService = workTaskService ?? throw new ArgumentNullException(nameof(workTaskService));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task NotifyTriangleTasks()
        {
            List<TaskMeasurement> tasksMeasurementsToNotify =
                await CollectTaskMeasurementsToNotify().ConfigureAwait(false);

            if (tasksMeasurementsToNotify.Count == 0)
            {
                mLogger.LogTrace("No tasks to notify");
                return;
            }

            mLogger.LogTrace($"Building report from {tasksMeasurementsToNotify.Count} tasks to notify about");

            string report = BuildReportFromTasksMeasurements(tasksMeasurementsToNotify);

            await mEmailService.SendEmail(report).ConfigureAwait(false);
        }

        private async Task<List<TaskMeasurement>> CollectTaskMeasurementsToNotify()
        {
            mLogger.LogDebug("Collecting tasks to notify");

            List<TaskMeasurement> tasksMeasurements = new List<TaskMeasurement>();

            IEnumerable<IWorkTask> tasks = await mWorkTaskService.FindWorkTasksByConditionAsync(
                task =>
                    !task.IsFinished).ConfigureAwait(false);

            foreach (IWorkTask task in tasks)
            {
                AddTaskMeasurementIfNeeded(task, tasksMeasurements);
            }

            return tasksMeasurements;
        }

        private static void AddTaskMeasurementIfNeeded(IWorkTask task, List<TaskMeasurement> tasksMeasurements)
        {
            TaskMeasurement taskMeasurement = new TaskMeasurement(task.ID, task.Description, task.TaskMeasurement);

            tasksMeasurements.Add(taskMeasurement);
        }

        private static string BuildReportFromTasksMeasurements(List<TaskMeasurement> tasksMeasurements)
        {
            StringBuilder reportBuilder = new StringBuilder();

            foreach (TaskMeasurement taskMeasurement in tasksMeasurements)
            {
                reportBuilder.Append("Task id: ").Append(taskMeasurement.Id).Append(" ")
                             .Append("Description: ").AppendLine(taskMeasurement.Description)
                             .AppendLine(taskMeasurement.Triangle.GetStringStatus())
                             .AppendLine("--------------------------------------------------");
            }

            return reportBuilder.ToString();
        }

        public async Task NotifySummary()
        {
            IEnumerable<IWorkTask> workTasks = await mWorkTaskService.FindWorkTasksByConditionAsync(
                task => !task.IsFinished).ConfigureAwait(false);

            string report = BuildReportFromGivenTasks(workTasks);

            await mEmailService.SendEmail(report).ConfigureAwait(false);
        }

        private static string BuildReportFromGivenTasks(IEnumerable<IWorkTask> workTasks)
        {
            StringBuilder reportBuilder = new StringBuilder();

            foreach (IWorkTask workTask in workTasks)
            {
                reportBuilder.Append("Task id: ").Append(workTask.ID).Append(' ')
                             .Append("Description: ").Append(workTask.Description).Append(' ')
                             .Append("Group: ").Append(workTask.GroupName).Append(' ')
                             .Append("Status: ").Append(workTask.Status)
                             .AppendLine();
            }

            return reportBuilder.ToString();
        }
    }
}