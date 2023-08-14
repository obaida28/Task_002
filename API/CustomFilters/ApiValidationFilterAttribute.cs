using API.ErrorResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.CustomFilters
{
    public class ApiValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                //Validation failed. Please check the input and correct any errors."
                context.Result = new BadRequestObjectResult(new ApiBadRequestResponse(context.ModelState));
            }

            base.OnActionExecuting(context);
        }
    }
}
