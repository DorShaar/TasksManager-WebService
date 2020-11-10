using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        private readonly ConcurrentDictionary<string, TaskMeasurementInfo> mOpenTasksMeasurementsDict =
            new ConcurrentDictionary<string, TaskMeasurementInfo>();

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
            await CleanFinishedTasksFromOpenTasksMeasurements().ConfigureAwait(false);

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

        private async Task CleanFinishedTasksFromOpenTasksMeasurements()
        {
            IEnumerable<IWorkTask> allClosedTasks =
                await mWorkTaskService.FindWorkTasksByConditionAsync(
                    task => task.IsFinished).ConfigureAwait(false);

            IEnumerable<string> allClosedTasksIds = allClosedTasks.Select(task => task.ID);

            HashSet<string> openTasksMeasurements = mOpenTasksMeasurementsDict.Keys.ToHashSet();

            HashSet<string> tasksIdsIntersection = openTasksMeasurements;

            tasksIdsIntersection.IntersectWith(allClosedTasksIds);

            foreach (string closedTaskId in tasksIdsIntersection)
            {
                if (mOpenTasksMeasurementsDict.TryRemove(closedTaskId, out TaskMeasurementInfo _))
                {
                    mLogger.LogDebug($"Task id {closedTaskId} removed from open tasks measurements dictionary");
                }
                else
                {
                    mLogger.LogWarning($"Task id {closedTaskId} was not removed from open tasks measurements dictionary" +
                        "although it was marked has should been removed");
                }
            }
        }

        private async Task<List<TaskMeasurement>> CollectTaskMeasurementsToNotify()
        {
            mLogger.LogDebug("Collecting tasks to notify");

            List<TaskMeasurement> tasksMeasurements = new List<TaskMeasurement>();

            IEnumerable<IWorkTask> tasks = await mWorkTaskService.FindWorkTasksByConditionAsync(
                task =>
                    !task.IsFinished &&
                    task.TaskMeasurement?.ShouldAlreadyBeNotified() == true).ConfigureAwait(false);

            foreach (IWorkTask task in tasks)
            {
                AddTaskMeasurementIfNeeded(task, tasksMeasurements);
            }

            return tasksMeasurements;
        }

        private void AddTaskMeasurementIfNeeded(IWorkTask task, List<TaskMeasurement> tasksMeasurements)
        {
            if (mOpenTasksMeasurementsDict.TryGetValue(task.ID, out TaskMeasurementInfo taskMeasurementInfo))
            {
                if (!taskMeasurementInfo.TaskMeasurement.Triangle.ShouldNotifyExact())
                    return;

                int currentProgressPercentage =
                    taskMeasurementInfo.TaskMeasurement.Triangle.GetCurrentTimeProgressPercentage();

                if (taskMeasurementInfo.PercentageProgress < currentProgressPercentage)
                    AddOrUpdateNewTaskMeasurement(taskMeasurementInfo, tasksMeasurements);

                return;
            }

            TaskMeasurement taskMeasurement = new TaskMeasurement(task.ID, task.Description, task.TaskMeasurement);
            taskMeasurementInfo = new TaskMeasurementInfo(taskMeasurement);
            AddOrUpdateNewTaskMeasurement(taskMeasurementInfo, tasksMeasurements);
        }

        private void AddOrUpdateNewTaskMeasurement(TaskMeasurementInfo taskMeasurementInfo,
            List<TaskMeasurement> TasksMeasurements)
        {
            mLogger.LogDebug($"Adding or updating task {taskMeasurementInfo.TaskMeasurement.Id} to dictionary");

            mOpenTasksMeasurementsDict.AddOrUpdate(
                taskMeasurementInfo.TaskMeasurement.Id, taskMeasurementInfo, (key, value) => value);

            TasksMeasurements.Add(taskMeasurementInfo.TaskMeasurement);
        }

        private string BuildReportFromTasksMeasurements(List<TaskMeasurement> tasksMeasurements)
        {
            StringBuilder reportBuilder = new StringBuilder();

            foreach (TaskMeasurement taskMeasurement in tasksMeasurements)
            {
                reportBuilder.Append("Task id: ").Append(taskMeasurement.Id).Append(" ")
                             .Append("Description: ").AppendLine(taskMeasurement.Description)
                             .AppendLine(taskMeasurement.Triangle.GetStatus())
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

        private string BuildReportFromGivenTasks(IEnumerable<IWorkTask> workTasks)
        {
            StringBuilder reportBuilder = new StringBuilder();

            foreach (IWorkTask workTask in workTasks)
            {
                reportBuilder.Append("Task id: ").Append(workTask.ID).Append(" ")
                             .Append("Description: ").Append(workTask.Description).Append(" ")
                             .Append("Group: ").Append(workTask.GroupName).Append(" ")
                             .Append("Status: ").Append(workTask.Status)
                             .AppendLine();
            }

            return reportBuilder.ToString();
        }
    }
}