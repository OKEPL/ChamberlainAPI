using Microsoft.AspNetCore.Mvc.Filters;

namespace Chamberlain.AppServer.Api.Endpoint.Infrastructure.Validation
#pragma warning disable 1591
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
                context.Result = new ValidationFailedResult(context.ModelState);
        }
    }
}