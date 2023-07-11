using Manager.Core.Models.Accounts;
using System.Linq.Expressions;

namespace Manager.Server.IServices
{
    public interface IAccountInfoService
    {
        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Task<AccountInfo?> FirstOrDefaultAsync(Expression<Func<AccountInfo, bool>> expression, bool isTrack = true);

        /// <summary>
        /// 修改账号信息表
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(AccountInfo accountInfo);

        /// <summary>
        ///查询用户信息【优先查询缓存】
        /// </summary>
        /// <param name="uId">guid:用户Id</param>
        /// <param name="isCache">false: 默认不缓存</param>
        /// <returns></returns>
        Task<AccountInfo?> FirstOrDefaultAsync(Guid? uId, bool isCache = false);
    }
}