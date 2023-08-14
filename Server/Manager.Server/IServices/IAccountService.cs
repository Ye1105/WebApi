using Manager.Core.Models.Accounts;
using System.Linq.Expressions;

namespace Manager.Server.IServices
{
    public interface IAccountService
    {
        /// <summary>
        /// 查询账号信息
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="password"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Task<Account> GetAccountBy(string phone, string password, bool isTrack = true);

        /// <summary>
        /// 获取账号
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Task<Account> GetAccountBy(Expression<Func<Account, bool>> expression, bool isTrack = true);

        /// <summary>
        /// 创建用户账号
        /// </summary>
        /// <param name="password"></param>
        /// <param name="phone"></param>
        /// <param name="nickName"></param>
        /// <param name="mail"></param>
        /// <returns></returns>
        Task<bool> CreateAccount(string password, string phone, string nickName, string mail);

        /// <summary>
        /// 更新用户账号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task<bool> ModifyAccount(Account account);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> ModifyAccountPassword(Expression<Func<Account, bool>> expression, string password);
    }
}