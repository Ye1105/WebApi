using Manager.Core.Models.Blogs;
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
    }
}