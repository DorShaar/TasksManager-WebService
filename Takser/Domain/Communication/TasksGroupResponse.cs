using TaskData.Contracts;

namespace Tasker.Domain.Communication
{
    public class TasksGroupResponse
    {
        private readonly BaseResponse mBaseResponse;

        public bool IsSuccess => mBaseResponse.IsSuccess;
        public string Message => mBaseResponse.Message;

        public ITasksGroup TasksGroup { get; private set; }

        private TasksGroupResponse(bool isSuccess, string message, ITasksGroup group)
        {
            mBaseResponse = new BaseResponse(isSuccess, message);
            TasksGroup = group;
        }

        public TasksGroupResponse(ITasksGroup group, string message) : this(true, message, group)
        {
        }

        public TasksGroupResponse(ITasksGroup group) : this(true, string.Empty, group)
        {
        }

        public TasksGroupResponse(string message) : this(false, message, null)
        {
        }
    }
}