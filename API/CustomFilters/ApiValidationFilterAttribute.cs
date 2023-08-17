﻿using API.ErrorResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.CustomFilters
{
    public class ApiValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
                context.Result = new BadRequestObjectResult(ApiBadRequestResponse.BADresponse(context.ModelState));
            base.OnActionExecuting(context);
        }
    }
}
