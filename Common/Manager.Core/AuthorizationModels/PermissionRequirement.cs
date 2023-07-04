using Manager.Core.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Manager.Core.AuthorizationModels
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 用户权限集合
        /// </summary>
        public IList<PermissionItem> Permissions { get; set; }

        /// <summary>
        /// 当前菜单路由的Crud权限
        /// sbyte 0: 启动  1: 禁止
        /// </summary>
        public Dictionary<CrudEnum, bool> Crud { get; set; }

        /// <summary>
        /// 自定义的 user claim
        /// </summary>
        //public Dictionary<string,string> UserClaims { get; set; }

        /// <summary>
        /// 无权限action
        /// </summary>
        public string DeniedAction { get; set; }

        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 订阅人
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan Expiration { get; set; }
    }
}