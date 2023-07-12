using Manager.Core.AuthorizationModels;
using Manager.Infrastructure.IRepositoies;
using Manager.JwtAuthorizePolicy.IServices;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace Manager.JwtAuthorizePolicy.Handler
{
    /// <summary>
    ///  https://blog.csdn.net/qq_25086397/article/details/103765090  自定义策略博客
    ///  https://docs.microsoft.com/zh-cn/aspnet/core/security/authorization/policies?view=aspnetcore-6.0  微软官网文档
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            dynamic? ct = context.Resource;
            if (ct == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            //IEnumerable<Claim> claims
            var claims = context.User.Claims;

            //账号id
            var idClaim = claims.FirstOrDefault(x => x.Type == Policys.Id);

            //用户id
            var uIdClaim = claims.FirstOrDefault(x => x.Type == Policys.UId);

            if (claims == null || !claims.Any() || idClaim == null || uIdClaim == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}