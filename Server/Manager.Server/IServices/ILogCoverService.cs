using Manager.Core.Models.Logs;
using Manager.Core.Page;

namespace Manager.Server.IServices
{
    public interface ILogCoverService
    {
        /// <summary>
        /// 新增封面
        /// </summary>
        /// <param name="logCover"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> AddAsync(LogCover logCover);

        /// <summary>
        /// 封面分页列表
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="offset"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<PagedList<LogCover?>?> PagedAsync(Guid uId, int pageIndex = 1, int pageSize = 10, int offset = 0, string orderBy = "");
    }
}