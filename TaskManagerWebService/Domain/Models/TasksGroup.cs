using System.Collections.Generic;

namespace MyFirstWebApp.Domain.Models
{
    public class TasksGroup
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public List<WorkTask> Tasks { get; set; }
    }
}