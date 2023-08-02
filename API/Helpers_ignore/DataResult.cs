using System.Net;

namespace API.Helpers;
public class DataResult<T> : Result, IDataResult<T>
    {
        public DataResult(T data, HttpStatusCode statusCode) : base(statusCode)
        {
            this.Data = data;
        }

        public DataResult(T data, HttpStatusCode statusCode, int totalRecords) : this(data, statusCode)
        {
            this.TotalRecords = totalRecords;
        }

        public DataResult(T data, HttpStatusCode statusCode, int totalRecords,string message) :base(statusCode, message)
        {
            this.Data = data;
            this.TotalRecords = totalRecords;
        }

        public DataResult(T data, HttpStatusCode statusCode, string message) : base(statusCode, message)
        {
            this.Data = data;
        }

        public T Data { get; }
        public int TotalRecords { get; }
    }