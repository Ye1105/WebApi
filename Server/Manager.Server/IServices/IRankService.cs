using Manager.Core.Models.Blogs;
using Manager.Core.Page;

namespace Manager.Server.IServices
{
    public interface IRankService
    {
        /// <summary>
        /// 热门榜单
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="offset"></param>
        /// <param name="isTrack"></param>
        /// <param name="id"></param>
        /// <param name="wId"></param>
        /// <returns></returns>
        Task<PagedList<Blog>?> GetPagedList(int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, Guid? id = null, Guid? wId = null);
    }
}