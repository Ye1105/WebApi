using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Server.IServices;
using Manager.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/users")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class UsersController : ApiController
    {
        private readonly IAccountInfoService accountInfoService;
        private readonly IAccountService accountService;
        private readonly IUserFocusService userFocusService;
        private readonly IBlogService blogService;

        public UsersController(IAccountInfoService accountInfoService, IAccountService accountService, IUserFocusService userFocusService, IBlogService blogService)
        {
            this.accountInfoService = accountInfoService;
            this.accountService = accountService;
            this.userFocusService = userFocusService;
            this.blogService = blogService;
        }


        /// <summary>
        /// 用户信息[当前登录用户]
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> UserInfo()
        {
            /*
             * 1.Account AccountInf
             * 2.博客数 关注数 粉丝数
             * 3.关注关系
             */
            var wId = UId;

            //1.Account AccountInfo
            var account = await accountService.GetAccountBy(x => x.UId == wId, false);

            var accountInfo = await accountInfoService.FirstOrDefaultAsync(wId, isCache: true);

            //2.1 博客数量
            var blogCount = await blogService.GetBlogCountBy(x => x.UId == wId && x.Status == (sbyte)Status.ENABLE);

            //2.2 关注数
            var focusCount = await userFocusService.GetUserFocusCountBy(x => x.UId == wId);

            //2.3 粉丝数
            var fanCount = await userFocusService.GetUserFocusCountBy(x => x.BuId == wId);

            return Ok(Success("用户信息获取成功", new { account, accountInfo, blogCount, focusCount, fanCount }));
        }


        /// <summary>
        /// 用户信息
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpGet("{uId}")]
        public async Task<IActionResult> UserInfo(Guid uId)
        {
            /*
             * 1.Account AccountInf
             * 2.博客数 关注数 粉丝数
             * 3.关注关系
             */
            var wId = UId;

            //1.Account AccountInfo
            //var account = await accountService.GetAccountBy(x => x.UId == uId, false);

            var accountInfo = await accountInfoService.FirstOrDefaultAsync(uId, false);

            //2.1 博客数量
            var blogCount = await blogService.GetBlogCountBy(x => x.UId == uId && x.Status == (sbyte)Status.ENABLE);

            //2.2 关注数
            var focusCount = await userFocusService.GetUserFocusCountBy(x => x.UId == uId);

            //2.3 粉丝数
            var fanCount = await userFocusService.GetUserFocusCountBy(x => x.BuId == uId);

            //3.关注关系
            var relation = await userFocusService.GetUserFocusBy(x => x.BuId == uId && x.UId == wId);

            return Ok(Success("用户信息获取成功", new { accountInfo, blogCount, focusCount, fanCount, relation }));
        }
    }
}