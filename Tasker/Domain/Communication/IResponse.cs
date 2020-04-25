namespace Tasker.Domain.Communication
{
    public interface IResponse<T> where T : class
    {
        T ResponseObject { get; }
        bool IsSuccess { get; }
        string Message { get; }
    }
}