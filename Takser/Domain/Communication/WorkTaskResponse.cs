using TaskData.Contracts;
using Tasker.Domain.Communication;

namespace Takser.Domain.Communication
{
    public class WorkTaskResponse
    {
        private readonly BaseResponse mBaseResponse;

        public bool IsSuccess => mBaseResponse.IsSuccess;
        public string Message => mBaseResponse.Message;

        public IWorkTask WorkTask { get; private set; }

        private WorkTaskResponse(bool isSuccess, string message, IWorkTask workTask)
        {
            mBaseResponse = new BaseResponse(isSuccess, message);
            WorkTask = workTask;
        }

        public WorkTaskResponse(IWorkTask workTask, string message) : this(true, message, workTask)
        {
        }

        public WorkTaskResponse(IWorkTask workTask) : this(true, string.Empty, workTask)
        {
        }

        public WorkTaskResponse(string message) : this(false, message, null)
        {
        }
    }
}