using Manager.Core.Models.Blogs;
using Manager.Core.Page;
using System.Linq.Expressions;

namespace Manager.Server.IServices
{
    public interface IBlogTopicService
    {
        /// <summary>
        /// 获取话题
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Task<BlogTopic> FirstOrDefaultAsync(Expression<Func<BlogTopic, bool>> expression, bool isTrack = true);

        /// <summary>
        /// 获取话题
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        BlogTopic FirstOrDefaultSync(Expression<Func<BlogTopic, bool>> expression, bool isTrack = true);

        /// <summary>
        /// 获取话题集合
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<List<BlogTopic>?> QueryAsync(Expression<Func<BlogTopic, bool>> expression, bool isTrack = true, string orderBy = "");

        /// <summary>
        /// 分页列表
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="offset"></param>
        /// <param name="isTrack"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<PagedList<BlogTopic>> PagedAsync(Expression<Func<BlogTopic, bool>> whereLambda, int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "");
    }
}