using Manager.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Diagnostics;

namespace Manager.API.Utility.Filters
{
    //https://www.bilibili.com/video/BV1Vd4y1t7er/?p=35&spm_id_from=pageDriver&vd_source=874ef91701c817855be9727acd96b7cd
    [AttributeUsage(AttributeTargets.All)]
    public class CustomLogAsyncActionFilterAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //包含了标记在当前要访问的Action上/当前Action所在的控制器上标记的所有的特性
            var meta = context.ActionDescriptor.EndpointMetadata;
            if (meta.Any(x => x.GetType().Equals(typeof(CustomAllowAnonymousAttribute))))
            {
                await next.Invoke();
                return;
            }

            var authorization = context.HttpContext.Request.Headers.Authorization;
            var arguments = context.ActionArguments.SerObj();
            var controllerName = context.HttpContext.GetRouteValue("controller").Str();
            var actionName = context.HttpContext.GetRouteValue("action");

            Log.Information("[OnActionExecuting]-[{0}]-[{1}]-[{2}]-[{3}]-[{4}]-[{5}]", controllerName, actionName, context.HttpContext.Request.Path, context.HttpContext.Request.ContentType, authorization, arguments);

            var watch = Stopwatch.StartNew();
            watch.Start();

            ActionExecutedContext excutedContext = await next.Invoke();  //此处执行Action

            var results = excutedContext.Result.SerObj();

            watch.Stop();

            Log.Information("[OnActionExecuted]-[{0}]-[{1}]-[{2}]-[{3}]-[{4}]", controllerName, actionName, context.HttpContext.Request.ContentType, results, watch.ElapsedMilliseconds);
        }
    }
}