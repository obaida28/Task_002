namespace API.ActionFilter;
public class ApiValidationFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(ApiResponse.BAD(context.ModelState));
        }
        base.OnActionExecuting(context);
    }
}