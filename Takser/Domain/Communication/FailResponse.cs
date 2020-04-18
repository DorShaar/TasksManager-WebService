namespace Tasker.Domain.Communication
{
    public class FailResponse<T> : IResponse<T> where T : class
    {
        public T ResponseObject { get; }

        public bool IsSuccess { get => false; }

        public string Message { get; }

        public FailResponse(T responseObject, string message)
        {
            ResponseObject = responseObject;
            Message = message;
        }

        public FailResponse(string message) : this(null, message)
        {
        }
    }
}