using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Core.RequestModels;
using Manager.Core.Settings;
using Manager.Extensions;
using Manager.JwtAuthorizePolicy;
using Manager.JwtAuthorizePolicy.IServices;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Manager.API.Controllers
{
    [ApiController]
    [Route("v1/api/tokens")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class TokensController : Controller
    {
        private readonly IAccountService accountService;
        private readonly IAccountInfoService accountInfoService;
        private readonly IAuthenticateService authenticateService;
        private readonly IJwtService jWTService;
        private readonly IOptions<AppSettings> appSettings;

        public TokensController(IAccountService accountService, IAccountInfoService accountInfoService, IAuthenticateService authenticateService, IJwtService jWTService, IOptions<AppSettings> appSettings)
        {
            this.accountService = accountService;
            this.accountInfoService = accountInfoService;
            this.authenticateService = authenticateService;
            this.jWTService = jWTService;
            this.appSettings = appSettings;
        }

        /// <summary>
        /// 更新访问令牌（同时也更新刷新令牌）
        /// </summary>
        /// <param name="req"></param>
        /// <param name="accessToken"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateAccessToken([FromBody] TokenRequest req)
        {
            /*
             * 1.先通过纯代码校验 refreshToken 的物理合法性
             * 2.从 RefreshToken 中获取 uId
             * 3.uId 和 RefreshToken ,判定对应的 redis hash 是否存在
             * 4.重新生成用户的accessToken,refreshToken
             */

            //1.先通过纯代码校验refreshToken的物理合法性
            string securityKey = appSettings.Value.JwtBearer.RefreshSecurityKey;
            var result = JwtHelper.JWTJieM(req.RefreshToken, securityKey);
            if (result == "expired" || result == "invalid" || result == "error" || result == "notvalid")
            {
                return Ok(ApiResult.Fail($"Token:{result}", "Token校验失败"));
            }

            //2.从 RefreshToken 中获取 uId
            dynamic myJwtData = JwtHelper.Base64UrlDecode(req.RefreshToken.Split('.')[1]).DesObj();
            Guid uId = myJwtData["uId"];
            Guid Id = myJwtData["id"];

            //3.uId 和 RefreshToken, 判定对应的 redis hash 是否存在
            var refreshToken = await jWTService.ExsitRefreshToken(uId, req.RefreshToken);
            if (!refreshToken.Item1)
            {
                return Ok(ApiResult.Fail(refreshToken.Item2));
            }

            //4.重新获取用户信息
            var account = await accountService.GetAccountBy(x => x.Id == Id, false);
            if (account != null)
            {
                /* 账号状态 */
                if (account.Status != (sbyte)Status.Enable)
                {
                    return Ok(ApiResult.Fail($"账号状态:{EnumDescriptionAttribute.GetEnumDescription((Status)account.Status)}"));
                }

                //5.重新生成accessToken和refreshToken，并写入user_refreshtoken redis表
                //5.1生成accessToken,refreshToken
                if (!authenticateService.IsAuthenticated(new AuthenticateRequest() { Id = account.Id, UId = account.UId }, out string AccessToken, out string RefreshToken))
                {
                    return Ok(ApiResult.Fail("账号认证失败"));
                }

                //将生成refreshToken的原始信息存到数据库/redis中
                var tokenRes = jWTService.AddRefreshToken(account.UId, RefreshToken);
                if (!tokenRes.Item1)
                {
                    return Ok(ApiResult.Fail(tokenRes.Item2));
                }

                var accountInfo = await accountInfoService.GetAccountInfoAndAvatarAndCoverById(account.UId, true);

                return Ok(ApiResult.Success("账号认证成功", new { account, accountInfo, AccessToken, RefreshToken }));
            }
            else
            {
                return Ok(ApiResult.Fail("账号不存在"));
            }
        }
    }
}