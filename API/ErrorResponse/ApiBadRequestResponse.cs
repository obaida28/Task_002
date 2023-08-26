namespace API.ErrorResponse
{
    public class ApiBadRequestResponse : ApiResponse
    {
        public IEnumerable<string> Errors { get; }
        private ApiBadRequestResponse(string message) : base(400,message) {}
        private ApiBadRequestResponse(ModelStateDictionary modelState)
            : base(400)
        {
            if (modelState.IsValid)
            {
                throw new ArgumentException("ModelState must be invalid", nameof(modelState));
            }

            Errors = modelState.SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage).ToArray();
        }
        public static ApiResponse BADresponse(ModelStateDictionary modelState) =>
            new ApiBadRequestResponse(modelState);
        public static ApiResponse BADresponse(string message) =>
            new ApiBadRequestResponse(message);
    }
}
