using Manager.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Manager.API.Utility.Filters
{
    [AttributeUsage(AttributeTargets.All)]
    public class CustomLogActionFilterAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// 在 Action 之前执行
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var arguments = context.ActionArguments.SerObj();
            var controllerName = context.HttpContext.GetRouteValue("controller");
            var actionName = context.HttpContext.GetRouteValue("action");

            Log.Information("[OnActionExecuting]-[{0}]-[{1}]-[{2}]-[{3}]", controllerName, actionName, context.HttpContext.Request.ContentType, arguments);
        }

        /// <summary>
        /// 在 Action 之后执行
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var results = context.Result.SerObj();
            var controllerName = context.HttpContext.GetRouteValue("controller");
            var actionName = context.HttpContext.GetRouteValue("action");

            Log.Information("[OnActionExecuted]-[{0}]-[{1}]-[{2}]-[{3}]", controllerName, actionName, context.HttpContext.Request.ContentType, results);
        }
    }
}