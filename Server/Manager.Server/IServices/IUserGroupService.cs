using Manager.Core.Models.Users;
using System.Linq.Expressions;

namespace Manager.Server.IServices
{
    public interface IUserGroupService
    {
        /// <summary>
        /// 获取用户分类【博客】
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Task<UserGroup?> FirstOrDefaultAsync(Expression<Func<UserGroup, bool>> expression, bool isTrack = true);

        /// <summary>
        /// 通过UId获取用户分类【博客】
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<UserGroup?> FirstOrDefaultAsync(Guid uId);

        /// <summary>
        /// 更新用户账号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(UserGroup userGroup);
    }
}