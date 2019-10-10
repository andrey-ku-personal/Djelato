using System;
using System.Collections.Generic;
using System.Text;

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
        public T Result { get; private set; }
        public string Message { get; private set; }

        public ServiceResult(bool isSuccessful, T result, string message)
        {
            IsSuccessful = isSuccessful;
            Result = result;
            Message = message;
        }
    }
}
