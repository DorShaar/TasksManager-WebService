using System;

namespace Tasker.Domain.Communication
{
    public class SuccessResponse<T> : IResponse<T> where T : class
    {
        public T ResponseObject { get; }

        public bool IsSuccess { get => true; }

        public string Message { get; }

        public SuccessResponse(T responseObject, string message = "")
        {
            ResponseObject = responseObject ?? throw new ArgumentException($"Parameter {nameof(responseObject)} cannot be null");
            Message = message;
        }
    }
}