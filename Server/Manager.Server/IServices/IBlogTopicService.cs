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
        Task<BlogTopic> GetBlogTopicBy(Expression<Func<BlogTopic, bool>> expression, bool isTrack = true);

        BlogTopic GetBlogTopicBySync(Expression<Func<BlogTopic, bool>> expression, bool isTrack = true);
    }
}