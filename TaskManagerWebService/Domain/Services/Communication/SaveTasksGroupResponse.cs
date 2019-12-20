using TaskManagerWebService.Domain.Models;

namespace TaskManagerWebService.Domain.Services.Communication
{
    public class SaveTasksGroupResponse
    {
        private readonly BaseResponse mBaseResponse;

        public bool IsSuccess => mBaseResponse.IsSuccess;
        public string Message => mBaseResponse.Message;

        public TasksGroup Group { get; private set; }

        private SaveTasksGroupResponse(bool isSuccess, string message, TasksGroup group)
        {
            mBaseResponse = new BaseResponse(isSuccess, message);
            Group = group;
        }

        public SaveTasksGroupResponse(TasksGroup group) : this(true, string.Empty, group)
        {
        }

        public SaveTasksGroupResponse(string message) : this(false, message, null)
        {
        }
    }
}