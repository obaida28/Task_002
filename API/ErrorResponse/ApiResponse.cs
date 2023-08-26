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

        public static ApiResponse response (int saveResult , object showResult = null) => 
            saveResult == 0 ? ApiBadRequestResponse.BADresponse("Bad Request") : ApiOkResponse.OKresponse(showResult);       

        public static ApiResponse response (object showResult = null) => ApiOkResponse.OKresponse(showResult);

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
