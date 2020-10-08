using System;
using Tasker.Domain.Models;

namespace Tasker.Infra.Services.Notifier
{
    internal class TaskMeasurementInfo
    {
        public TaskMeasurement TaskMeasurement { get; }
        public int PercentageProgress { get; }

        public TaskMeasurementInfo(TaskMeasurement taskMeasurement)
        {
            TaskMeasurement = taskMeasurement ?? throw new ArgumentNullException(nameof(taskMeasurement));
            PercentageProgress = TaskMeasurement.Triangle.GetCurrentTimeProgressPercentage();
        }
    }
}