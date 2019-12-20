using System.Collections.Generic;

namespace TaskManagerWebService.Domain.Models
{
    public class TasksGroup
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public List<WorkTask> Tasks { get; set; }
    }
}