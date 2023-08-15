using Newtonsoft.Json;

namespace API.ErrorResponse
{
    public class ApiResponse
    {
        public int StatusCode { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; }

        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        public static ApiResponse response (int saveResult , object showResult) => 
            saveResult == 0 ? new ApiBadRequestResponse("Bad Request") : new ApiOkResponse(showResult);       

        public static ApiResponse response (int saveResult) => 
            saveResult == 0 ? new ApiBadRequestResponse("Bad Request") : new ApiOkResponse();

        private static string GetDefaultMessageForStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return "Resource not found";
                case 500:
                    return "An unhandled error occurred";
                default:
                    return null;
            }
        }
    }
}
