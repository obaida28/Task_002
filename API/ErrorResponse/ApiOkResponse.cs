namespace API.ErrorResponse
{
    public class ApiOkResponse : ApiResponse
    {
        public object? Result { get; }
        private ApiOkResponse() : base(200) => Result = null;
        private ApiOkResponse(object result) : base(200) => Result = result;
        public static ApiResponse OKresponse(object result = null) => new ApiOkResponse(result);
    }
}
