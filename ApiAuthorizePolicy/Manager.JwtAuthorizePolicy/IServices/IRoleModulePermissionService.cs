using Manager.Core.AuthorizationModels;
using Manager.Core.Models.Accounts;
using System.Linq.Expressions;

namespace Manager.JwtAuthorizePolicy.IServices
{
    public interface IRoleModulePermissionService
    {
        /// <summary>
        /// 获取用户的角色列表
        /// </summary>
        /// <param name="express"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Task<List<AccountRole>> GetAccountRolesAsync(Expression<Func<AccountRole, bool>> express, bool isTrack = true);

        /// <summary>
        /// 获取 role_permission 列表和  module_info 列表
        /// </summary>
        /// <returns></returns>
        Task<RolePermissionViewModel?> GetRolePermissionAsync();
    }
}