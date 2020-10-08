using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
using Tasker.App.Services;
using Tasker.Domain.Models;

namespace Tasker.Infra.Services.Notifier
{
    public class NotifierService : INotifierService
    {
        private readonly ILogger<NotifierService> mLogger;
        private readonly IEmailService mEmailService;
        private readonly ITasksGroupService mTasksGroupService;

        private readonly ConcurrentDictionary<string, TaskMeasurementInfo> mOpenTasksMeasurementsDict =
            new ConcurrentDictionary<string, TaskMeasurementInfo>();

        public NotifierService(IEmailService emailService,
            ITasksGroupService tasksGroupService,
            ILogger<NotifierService> logger)
        {
            mEmailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            mTasksGroupService = tasksGroupService ?? throw new ArgumentNullException(nameof(tasksGroupService));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Notify()
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
            List<TaskMeasurement> tasksMeasurements = new List<TaskMeasurement>();

            IEnumerable<ITasksGroup> groups = await mTasksGroupService.ListAsync().ConfigureAwait(false);

            foreach (ITasksGroup group in groups)
            {
                IEnumerable<IWorkTask> tasks = group.GetAllTasks();

                foreach (IWorkTask task in tasks)
                {
                    if (task.IsFinished ||
                        task.TaskMeasurement == null ||
                        !task.TaskMeasurement.ShouldAlreadyBeNotified())
                    {
                        continue;
                    }

                    AddTaskMeasurement(task, tasksMeasurements);
                }
            }

            return tasksMeasurements;
        }

        private void AddTaskMeasurement(IWorkTask task, List<TaskMeasurement> tasksMeasurements)
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
                             .Append("Description: ").Append(taskMeasurement.Description)
                             .AppendLine()
                             .AppendLine(taskMeasurement.Triangle.GetStatus())
                             .AppendLine("--------------------------------------------------");
            }

            return reportBuilder.ToString();
        }
    }
}