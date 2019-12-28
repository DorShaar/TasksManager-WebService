using TaskManagerWebService.Domain.Models;

namespace TaskManagerWebService.Domain.Services.Communication
{
    public class TasksGroupResponse
    {
        private readonly BaseResponse mBaseResponse;

        public bool IsSuccess => mBaseResponse.IsSuccess;
        public string Message => mBaseResponse.Message;

        public TasksGroup Group { get; private set; }

        private TasksGroupResponse(bool isSuccess, string message, TasksGroup group)
        {
            mBaseResponse = new BaseResponse(isSuccess, message);
            Group = group;
        }

        public TasksGroupResponse(TasksGroup group, string message) : this(true, message, group)
        {
        }

        public TasksGroupResponse(TasksGroup group) : this(true, string.Empty, group)
        {
        }

        public TasksGroupResponse(string message) : this(false, message, null)
        {
        }
    }
}