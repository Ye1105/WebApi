using Manager.Core.Models.Blogs;
using Manager.Core.Page;

namespace Manager.Server.IServices
{
    public interface IBlogLikeService
    {
        /// <summary>
        /// 博客：用户点赞
        /// </summary>
        /// <param name="bId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> AddAsync(Guid bId, Guid uId);

        /// <summary>
        /// 博客：用户取消赞
        /// </summary>
        /// <param name="bId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>

        Task<Tuple<bool, string>> DeleteAsync(Guid bId, Guid uId);

        /// <summary>
        /// 博客点赞数
        /// </summary>
        /// <param name="bId"></param>
        /// <returns></returns>
        Task<long?> CountAsync(Guid bId);

        /// <summary>
        /// 当前用户是否点赞博客
        /// </summary>
        /// <param name="bId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<bool?> ExsitAsync(Guid bId, Guid uId);

        /// <summary>
        /// 点赞博客分页列表
        /// </summary>
        /// <param name="wId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Task<PagedList<Blog?>?> PagedAsync(Guid wId, int pageIndex = 1, int pageSize = 10, int offset = 0);
    }
}