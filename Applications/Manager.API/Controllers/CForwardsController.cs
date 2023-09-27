using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Core.Models.Blogs;
using Manager.Core.RequestModels;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/cforwards")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class CForwardsController : ApiController
    {
        private readonly IBlogService blogService;

        public CForwardsController(IBlogService blogService)
        {
            this.blogService = blogService;
        }

    }
}