using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.ErrorResponse
{
    public class ApiBadRequestResponse : ApiResponse
    {
        public IEnumerable<string> Errors { get; }
        public ApiBadRequestResponse(string message) : base(400,message) {}
        public ApiBadRequestResponse(ModelStateDictionary modelState)
            : base(400)
        {
            if (modelState.IsValid)
            {
                throw new ArgumentException("ModelState must be invalid", nameof(modelState));
            }

            Errors = modelState.SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage).ToArray();
        }
    }
}
