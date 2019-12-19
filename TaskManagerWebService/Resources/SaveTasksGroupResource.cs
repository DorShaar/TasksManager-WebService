using System.ComponentModel.DataAnnotations;

namespace TaskManagerWebService.Resources
{
    public class SaveTasksGroupResource
    {
        [Required]
        public string GroupName { get; set; }
    }
}