using System.Collections.Generic;

namespace Tasker.Domain.Models
{
    public class TasksGroup
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public List<WorkTask> Tasks { get; set; }
    }
}