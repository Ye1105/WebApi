using Manager.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Manager.API.Utility.Filters
{
    [AttributeUsage(AttributeTargets.All)]
    public class CustomExceptionFilterAttribute : Attribute, IExceptionFilter, IAsyncExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            throw new NotImplementedException();
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.ExceptionHandled == false)
            {
                var controllerName = context.HttpContext.GetRouteValue("controller");
                var actionName = context.HttpContext.GetRouteValue("action");

                //断路器 --  只要对Result赋值，就不继续往后
                Log.Error("[CustomExceptionFilter]-[{0}]-[{1}] => {2}", controllerName, actionName, context.Exception);
                context.Result = new JsonResult(ApiController.Fail(context.Exception, $"[{controllerName}] - [{actionName}] - 异常", "数据异常"));

                //表示当前异常被处理过
                context.ExceptionHandled = true;
            }

            return Task.CompletedTask;
        }
    }
}