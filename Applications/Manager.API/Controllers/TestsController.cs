using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.AuthorizationModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("v1/api/tests")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    [TypeFilter(typeof(CustomLogAsyncActionFilterAttribute))]
    public class TestsController : ApiController
    {
        //[CustomSelect]
        //public IAccountInfoService accountInfoServiceProp { get; set; }

        [HttpPost("test")]
        [Authorize(Policy = Policys.VIP)]
        public IActionResult TestPermisson()
        {
            //var ac = accountInfoServiceProp;

            return Ok(Success("v1test"));
            //throw new Exception("test error");
        }
    }
}