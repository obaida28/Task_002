using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.ErrorResponse
{
    public class ApiNotFoundResponse : ApiResponse
    {
        public IEnumerable<string> Errors { get; }
        public ApiNotFoundResponse(string message) : base(404,message) {}
    }
}
