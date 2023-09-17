using Manager.Core.Models.Logs;
using Manager.Core.Page;

namespace Manager.Server.IServices
{
    public interface ILogAvatarService
    {
        /// <summary>
        /// 头像分页列表
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="offset"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<PagedList<LogAvatar?>?> PagedAsync(Guid uId, int pageIndex = 1, int pageSize = 10, int offset = 0, string orderBy = "");

        /// <summary>
        /// 新增头像
        /// </summary>
        /// <param name="logAvatar"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> AddAsync(LogAvatar logAvatar);
    }
}