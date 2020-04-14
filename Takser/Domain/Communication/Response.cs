namespace Tasker.Domain.Communication
{
    public class Response<T> where T : class
    {
        public bool IsSuccess { get; }
        public string Message { get; }

        public T ResponseObject { get; private set; }

        private Response(bool isSuccess, string message, T responseObject)
        {
            IsSuccess = isSuccess;
            Message = message;
            ResponseObject = responseObject;
        }

        public Response(T responseObject, bool isSuccess, string message) : this(isSuccess, message, responseObject)
        {
        }

        public Response(T responseObject, bool isSuccess) : this(isSuccess, string.Empty, responseObject)
        {
        }

        public Response(bool isSuccess, string message) : this(isSuccess, message, null)
        {
        }
    }
}