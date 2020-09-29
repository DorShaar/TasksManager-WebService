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
using Triangle;

namespace Tasker.Infra.Services
{
    public class NotifierService : INotifierService
    {
        private readonly ILogger<NotifierService> mLogger;
        private readonly IEmailService mEmailService;
        private readonly ITasksGroupService mTasksGroupService;

        private readonly ConcurrentDictionary<int, TaskMeasurement> mOpenTasksMeasurements =
            new ConcurrentDictionary<int, TaskMeasurement>();

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
            List<TaskMeasurement> TasksMeasurements = new List<TaskMeasurement>();

            IEnumerable<ITasksGroup> groups = await mTasksGroupService.ListAsync().ConfigureAwait(false);

            foreach (ITasksGroup group in groups)
            {
                IEnumerable<IWorkTask> tasks = group.GetAllTasks();

                foreach (IWorkTask task in tasks)
                {
                    if (task.TaskMeasurement == null)
                        continue;

                    if (task.TaskMeasurement.ShouldNotify() && CheckIsAlreadyNotified(task.ID, task.TaskMeasurement))
                    {
                        TasksMeasurements.Add(new TaskMeasurement(task.ID, task.Description, task.TaskMeasurement));
                        UpdateAlreadyNotified(task.ID, task.TaskMeasurement);
                    }
                }
            }

            return TasksMeasurements;
        }

        private bool CheckIsAlreadyNotified(string id, TaskTriangle taskMeasurement)
        {

        }

        private void UpdateAlreadyNotified(string id, TaskTriangle taskMeasurement)
        {
            fghfgh
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