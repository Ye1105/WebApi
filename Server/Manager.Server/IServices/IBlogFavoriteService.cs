using Manager.Core.Models.Blogs;
using Manager.Core.Page;

namespace Manager.Server.IServices
{
    public interface IBlogFavoriteService
    {
        /// <summary>
        ///  博客收藏：增加收藏
        /// </summary>
        /// <param name="bId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> AddAsync(Guid bId, Guid uId);

        /// <summary>
        ///  博客收藏：删除收藏
        /// </summary>
        /// <param name="bId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> DeleteAsync(Guid bId, Guid uId);

        /// <summary>
        /// 收藏数量
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Task<long?> CountAsync(Guid bId);

        /// <summary>
        /// 当前用户是否收藏博客
        /// </summary>
        /// <param name="bId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<bool?> ExsitAsync(Guid bId, Guid uId);

        /// <summary>
        /// 收藏博客分页列表
        /// </summary>
        /// <param name="wId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="offset"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Task<PagedList<Blog?>?> PagedAsync(Guid wId, int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true);
    }
}