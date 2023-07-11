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
    [Route("v2/api/tests")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V2))]
    [CustomExceptionFilter]
    public class TestsV2Controller : ApiController
    {
        //[CustomSelect]
        //public IAccountInfoService accountInfoServiceProp { get; set; }

        [HttpPost("test")]
        [Authorize(Policy = Policys.API)]
        public IActionResult TestPermisson()
        {
            //var ac = accountInfoServiceProp;
            return Ok(Success("v2test"));

            //throw new Exception("test error");
        }
    }
}