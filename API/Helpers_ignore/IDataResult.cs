namespace API.Helpers;
public interface IDataResult<out T> : IResult
    {
        T Data { get; }
        int TotalRecords { get; }
    }