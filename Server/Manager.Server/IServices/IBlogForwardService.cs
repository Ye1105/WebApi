using Manager.Core.Enums;
using Manager.Core.Models.Blogs;
using Manager.Core.Page;
using System.Linq.Expressions;

namespace Manager.Server.IServices
{
    public interface IBlogForwardService
    {
        /// <summary>
        /// 获取转发数量
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Task<int> GetBlogForwardCountBy(Expression<Func<BlogForward, bool>> expression, bool isTrack = true);

        Task<PagedList<BlogForward>?> GetPagedList(int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "", Guid? id = null, Guid? uId = null, Guid? baseBId = null, Guid? prevBId = null, Guid? buId = null, Guid? prevCId = null, DateTime? startTime = null, DateTime? endTime = null, Guid? wId = null, ForwardScope? scope = null, Status? status = null);

        /// <summary>
        /// 博客转发
        /// </summary>
        /// <param name="blog"></param>
        /// <param name="blogForward"></param>
        /// <returns></returns>
        Task<bool> AddBlogForward(Blog blog, BlogForward blogForward);

        /// <summary>
        /// 删除转发
        /// </summary>
        /// <param name="blog"></param>
        /// <param name="blogForward"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> DeleteBlogForward(Guid id, Guid uId);
    }
}