﻿using Manager.Core.Models.Accounts;
using Manager.Core.Page;
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
        ///查询用户信息【优先查询缓存】
        /// </summary>
        /// <param name="uId">guid:用户Id</param>
        /// <param name="isCache">false: 默认不缓存</param>
        /// <returns></returns>
        Task<AccountInfo?> FirstOrDefaultAsync(Guid? uId, bool isCache = false);

        /// <summary>
        /// 修改账号信息表
        /// </summary>
        /// <param name="accountInfo"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(AccountInfo accountInfo);

        /// <summary>
        /// 分页列表
        /// </summary>
        /// <typeparam name="TaccountInfo"></typeparam>
        /// <param name="whereLambda"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="offset"></param>
        /// <param name="isTrack"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<PagedList<AccountInfo>> PagedAsync(Expression<Func<AccountInfo, bool>> whereLambda, int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "");
    }
}