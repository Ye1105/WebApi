using Manager.API.Utility.Filters;
using Manager.API.Utility;
using Manager.Core;
using Microsoft.AspNetCore.Mvc;
using Manager.Core.Page;
using Manager.Server.Services;

namespace Manager.API.Controllers
{
    /// <summary>
    /// 话题控制器
    /// </summary>
    [ApiController]
    [Route("v1/api/topics")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class TopicsController : ApiController
    {

        [HttpGet("match")]
        public async Task<IActionResult> Match([FromQuery] string search)
        {
            return Ok();
        }
    }
}
