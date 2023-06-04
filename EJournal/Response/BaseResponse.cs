using Microsoft.AspNetCore.Http;

namespace EJournal.Response
{
    public class BaseResponse<T> : IBaseResponse<T>
    {
        public string Description { get; set; }

        public StatusCodeEnum StatusCode { get; set; }

        public T Data { get; set; }
    }

    public interface IBaseResponse<T>
    {

        StatusCodeEnum StatusCode { get; }
        T Data { get; }
    }
    public enum StatusCodeEnum
    {
        AccountNotFound = 0,

        ThisAccountHasBeenActivated = 10,
        Unauthorized = 401,
        OK = 200,
        InternalServerError = 500
    }
}
