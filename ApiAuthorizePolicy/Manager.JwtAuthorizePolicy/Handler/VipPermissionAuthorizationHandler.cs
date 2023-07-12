using Manager.Core.AuthorizationModels;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;

namespace Manager.JwtAuthorizePolicy.Handler
{
    /// <summary>
    ///  https://blog.csdn.net/qq_25086397/article/details/103765090  自定义策略博客
    ///  https://docs.microsoft.com/zh-cn/aspnet/core/security/authorization/policies?view=aspnetcore-6.0  微软官网文档
    /// </summary>
    public class VipPermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IAccountInfoService accountInfoService;

        public VipPermissionAuthorizationHandler(IAccountInfoService accountInfoService)
        {
            this.accountInfoService = accountInfoService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            dynamic? ct = context.Resource;
            if (ct == null)
            {
                context.Fail();
                return;
            }

            //IEnumerable<Claim> claims
            var claims = context.User.Claims;

            //用户id
            var uId = claims.FirstOrDefault(x => x.Type == Policys.UId);

            if (claims == null || uId == null)
            {
                context.Fail();
                return;
            }

            //获取用户是否是VIP
            var accountInfo = await accountInfoService.FirstOrDefaultAsync(x => x.UId == Guid.Parse(uId.Value), isTrack: false);
            if (accountInfo != null && accountInfo.Vip > 0)
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
            return;
        }
    }
}