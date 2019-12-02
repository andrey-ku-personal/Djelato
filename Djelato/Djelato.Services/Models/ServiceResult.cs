namespace Djelato.Services.Models
{
    public struct ServiceResult
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; private set; }

        public ServiceResult(bool isSuccessful, string message)
        {
            IsSuccessful = isSuccessful;
            Message = message;
        }
    }

    public struct ServiceResult<T>
    {
        public bool IsSuccessful { get; set; }
        public T Obj { get; private set; }
        public string Message { get; private set; }

        public ServiceResult(bool isSuccessful, T obj, string message)
        {
            IsSuccessful = isSuccessful;
            Obj = obj;
            Message = message;
        }
    }
}
