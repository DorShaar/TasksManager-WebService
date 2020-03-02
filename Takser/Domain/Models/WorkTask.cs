namespace Tasker.Domain.Models
{
    public class WorkTask
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public TasksGroup ParentGroup { get; set; }
    }
}