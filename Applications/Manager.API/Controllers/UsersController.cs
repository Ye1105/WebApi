using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.AuthorizationModels;
using Manager.Core.Enums;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    //[Authorize(Policy = Policys.VIP)]
    [Authorize]
    [ApiController]
    [Route("v1/api/users")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    //[TypeFilter(typeof(CustomLogAsyncActionFilterAttribute))]
    [CustomExceptionFilter]
    public class UsersController : ApiController
    {
        private readonly IAccountInfoService accountInfoService;
        private readonly IUserFocusService userFocusService;
        private readonly IBlogService blogService;

        public UsersController(IAccountInfoService accountInfoService, IUserFocusService userFocusService, IBlogService blogService)
        {
            this.accountInfoService = accountInfoService;
            this.userFocusService = userFocusService;
            this.blogService = blogService;
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="wId"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        [HttpGet("{uId}/{wId?}")]
        public async Task<IActionResult> GetUser(Guid uId, Guid? wId)
        {
            var uid = UId;
            /*
             * 1.Account AccountInf
             * 2.博客数 关注数 粉丝数
             * 3.关注关系
             */

            //1.Account AccountInfo
            //var account = await accountService.GetAccountBy(x => x.UId == uId, false);

            var accountInfo = await accountInfoService.FirstOrDefaultAsync(uId, true);

            //2.1 博客数量
            var blogCount = await blogService.GetBlogCountBy(x => x.UId == uId && x.Status == (sbyte)Status.ENABLE);

            //2.2 关注数
            var focusCount = await userFocusService.GetUserFocusCountBy(x => x.UId == uId);

            //2.3 粉丝数
            var fanCount = await userFocusService.GetUserFocusCountBy(x => x.BuId == uId);

            //3.关注关系
            var relation = wId != null && wId != Guid.Empty ? await userFocusService.GetUserFocusBy(x => x.BuId == uId && x.UId == wId) : null;

            return Ok(ApiController.Success("账号和用户信息获取成功", new { accountInfo, blogCount, focusCount, fanCount, relation }));
        }
    }
}