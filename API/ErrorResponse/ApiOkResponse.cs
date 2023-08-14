namespace API.ErrorResponse
{
    public class ApiOkResponse : ApiResponse
    {
        public object? Result { get; }

        public ApiOkResponse()
            : base(200)
        {
            Result = null;
        }
        public ApiOkResponse(object result)
            : base(200)
        {
            Result = result;
        }
    }
}
