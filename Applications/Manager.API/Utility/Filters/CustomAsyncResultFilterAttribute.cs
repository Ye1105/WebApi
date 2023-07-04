using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Manager.API.Utility.Filters
{
    [AttributeUsage(AttributeTargets.All)]
    public class CustomAsyncResultFilterAttribute : Attribute, IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            //可以整合返回值
            if (context.Result is JsonResult result)
            {
                context.Result = new JsonResult(new
                {
                    Data = result.Value
                });
            }
            await next.Invoke();
        }
    }
}