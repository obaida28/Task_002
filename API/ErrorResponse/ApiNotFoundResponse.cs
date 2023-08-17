using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.ErrorResponse
{
    public class ApiNotFoundResponse : ApiResponse
    {
        public IEnumerable<string> Errors { get; }
        private ApiNotFoundResponse(string message) : base(404,message) {}
        public static ApiResponse NOTresponse(string message) =>
            new ApiNotFoundResponse(message);
    }
}
