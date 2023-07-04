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
        Task<AccountInfo?> GetAccountInfoBy(Expression<Func<AccountInfo, bool>> expression, bool isTrack = true);

        /// <summary>
        /// 修改账号信息表
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <returns></returns>
        Task<bool> ModifyAccountInfo(AccountInfo accountInfo);

        /// <summary>
        ///查询用户信息【优先查询缓存】
        /// </summary>
        /// <param name="uId">用户Id</param>
        /// <param name="isCache">默认不缓存</param>
        /// <returns></returns>
        Task<AccountInfo?> GetAccountInfoAndAvatarAndCoverById(Guid? uId, bool isCache = false);
    }
}