using Triangle;

namespace Tasker.Domain.Models
{
    public class TaskMeasurement
    {
        public string Id { get; }
        public string Description { get; }
        public TaskTriangle Triangle { get; }

        public TaskMeasurement(string id, string description, TaskTriangle triangle)
        {
            Id = id;
            Description = description;
            Triangle = triangle;
        }
    }
}