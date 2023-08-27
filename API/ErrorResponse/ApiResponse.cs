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

        public static ApiResponse Response (int saveResult , object showResult = null) => 
            saveResult == 0 ? BAD("Bad Request") : OK(showResult);       

        public static ApiResponse Response (object showResult = null) => OK(showResult);
        /// <summary>
        /// API OK Response 200
        /// </summary>
        /// <param name="showResult"></param>
        /// <returns></returns>
        public static ApiResponse OK(object showResult = null) => new ApiOkResponse(showResult);
        public static ApiResponse BAD(ModelStateDictionary modelState) => new ApiBadRequestResponse(modelState);
        /// <summary>
        /// API Bad Request 400
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ApiResponse BAD(string message) => new ApiBadRequestResponse(message);
        /// <summary>
        /// API Not Found 404
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ApiResponse NOT(string message) => new ApiNotFoundResponse(message);
        
        public static object GetResult(ApiResponse response)
        {
            if(response is ApiOkResponse okResponse)
                return okResponse.Result;
            return null;
        }

        private static string? GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                404 => "Resource not found",
                500 => "An unhandled error occurred",
                _ => null,
            };

        }
        
        private class ApiOkResponse : ApiResponse
        {
            public object? Result { get; }
            public ApiOkResponse() : base(200) => Result = null;
            public ApiOkResponse(object result) : base(200) => Result = result;
            //public static ApiResponse OKresponse(object result = null) => new ApiOkResponse(result);
        }
        private class ApiBadRequestResponse : ApiResponse
        {
            public IEnumerable<string> Errors { get; }
            public ApiBadRequestResponse(string message) : base(400,message) {}
            public ApiBadRequestResponse(ModelStateDictionary modelState) : base(400)
            {
                if (modelState.IsValid)
                    throw new ArgumentException("ModelState must be invalid", nameof(modelState));
                Errors = modelState.SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage).ToArray();
            }
        }
        private class ApiNotFoundResponse : ApiResponse
        {
            public IEnumerable<string> Errors { get; }
            public ApiNotFoundResponse(string message) : base(404,message) {}
        }
    }
}
