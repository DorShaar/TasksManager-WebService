namespace TaskManagerWebService.Domain.Services.Communication
{
    public class BaseResponse
    {
        public bool IsSuccess { get; }
        public string Message { get; }

        public BaseResponse(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}