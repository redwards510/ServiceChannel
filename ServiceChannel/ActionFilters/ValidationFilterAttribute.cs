using Microsoft.AspNetCore.Mvc.Filters;
namespace ActionFilters.ActionFilters
{
    public class ValidationFilterAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {

            //var param = context.ActionArguments.SingleOrDefault(p => p.Value is IEntity);
            //if (param.Value == null)
            //{
            //    context.Result = new BadRequestObjectResult("Object is null");
            //    return;
            //}

            //if (!context.ModelState.IsValid)
            //{
            //    context.Result = new BadRequestObjectResult(context.ModelState);
            //}

        }
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
