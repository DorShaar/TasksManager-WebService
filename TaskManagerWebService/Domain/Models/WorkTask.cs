namespace MyFirstWebApp.Domain.Models
{
    public class WorkTask
    {
        public string TaskId { get; set; }
        public string Name { get; set; }

        public string GroupId { get => ParentGroup.GroupId; set => GroupId = value; }
        public TasksGroup ParentGroup { get; set; }
    }
}