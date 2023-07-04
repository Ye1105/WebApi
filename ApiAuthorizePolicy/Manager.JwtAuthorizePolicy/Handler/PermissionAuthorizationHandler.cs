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
        private readonly IBase baseService;
        private readonly IRoleModulePermissionService roleModulePermissionService;

        public PermissionAuthorizationHandler(IBase baseService, IRoleModulePermissionService roleModulePermissionService)
        {
            this.baseService = baseService;
            this.roleModulePermissionService = roleModulePermissionService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            try
            {
                var c = context;

                var result = context.FailureReasons;

                dynamic? ct = context.Resource;
                if (ct == null)
                {
                    context.Fail();
                    return;
                }

                //Path
                var path = ct.Request.Path;

                var routeValues = ct.Request.RouteValues;

                //Controller
                var controller = routeValues["controller"];

                //Action
                var action = routeValues["action"];

                //IEnumerable<Claim> claims
                var claims = context.User.Claims;

                //账号id
                var idClaim = claims.FirstOrDefault(x => x.Type == Policys.Id);

                //用户id
                var uIdClaim = claims.FirstOrDefault(x => x.Type == Policys.UId);

                if (claims == null || !claims.Any() || idClaim == null || uIdClaim == null)
                {
                    context.Fail();
                    return;
                }

                /*
                 * 1. 获取角色和接口列表，可以用 AOP ,项目中使用 redis 缓存
                 * 2. 当前用户的状态
                 * 3. 当前用户的角色
                 * 4. 当前用户的角色对应有权限的的所有 module
                 * 5. 判断模块中是否有相同 Controller 和  Action 的模块
                 *
                 */

                //获取当前的用户角色列表
                var aR = await roleModulePermissionService.GetAccountRolesAsync(x => x.UId == Guid.Parse(uIdClaim.Value), false);

                if (aR == null || !aR.Any())
                {
                    context.Fail();
                    return;
                }

                var rList = aR.Select(x => x.RId).ToList();

                //获取 role_permission 和 module_info
                var pR = await roleModulePermissionService.GetRolePermissionAsync();

                if (pR == null || pR.RolePermissions == null || pR.ModuleInfos == null)
                {
                    context.Fail();
                    return;
                }

                //当前用户对应的所有【角色】在 Policys.Permission 的所有权限路由=> controller action 的匹配项
                var res = (from r in pR.RolePermissions
                           join m in pR.ModuleInfos
                           on r.MId equals m.Id
                           where
                             rList.Contains(r.RId)
                             && m.Controller == controller
                             && m.Action == action
                           select m).ToList();

                if (res == null || !res.Any())
                {
                    context.Fail();
                    return;
                }

                context.Succeed(requirement);
                return;
            }
            catch (Exception ex)
            {
                //var failReason = new AuthorizationFailureReason(this, "vip策略校验失败");
                //context.Fail(failReason);
                Log.Error("HandleRequirementAsync error {0}", ex);
                context.Fail(new AuthorizationFailureReason(this, ex.ToString()));
                return;
            }
        }
    }
}