namespace API.Helpers;
public interface IResult
{
  int StatusCode { get; }
  string Message { get;  }
}