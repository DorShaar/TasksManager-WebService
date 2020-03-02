using System.ComponentModel.DataAnnotations;

namespace Tasker.App.Resources
{
    public class SaveTasksGroupResource
    {
        [Required]
        public string GroupName { get; set; }
    }
}