using Manager.Core.Enums;
using Manager.Core.Models.Blogs;
using Manager.Core.Models.Users;
using Manager.Core.Page;
using System.Linq.Expressions;

namespace Manager.Server.IServices
{
    public interface IBlogService
    {
        /// <summary>
        /// 发表博客
        /// </summary>
        /// <param name="blog"></param>
        /// <param name="blogTopic"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task<bool> AddAsync(Blog blog, List<BlogTopic>? blogTopic, UserTopic? topic);

        /// <summary>
        /// 发表博客
        /// </summary>
        /// <param name="blog"></param>
        /// <param name="blogTopic"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        bool AddSync(Blog blog, List<BlogTopic>? blogTopic, UserTopic? topic);

        /// <summary>
        /// 获取博客
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Task<Blog?> FirstOrDefaultAsync(Expression<Func<Blog, bool>> expression, bool isTrack = true);

        /// <summary>
        /// 获取博客
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Blog? FirstOrDefaultSync(Expression<Func<Blog, bool>> expression, bool isTrack = true);

        /// <summary>
        /// 更新博客
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(Blog blog);

        /// <summary>
        /// 返回符合的数据条数
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<Blog, bool>> expression, bool isTrack = true);

        /// <summary>
        /// 返回符合的数据条数
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        int CountSync(Expression<Func<Blog, bool>> expression, bool isTrack = true);

        /// <summary>
        /// 查询博客分页列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="offset">偏移量</param>
        /// <param name="isTrack">是否跟踪：默认 ture</param>
        /// <param name="orderBy">排序</param>
        /// <param name="id">博客id</param>
        /// <param name="wId">当前网站登录的用户id</param>
        /// <param name="uId">发表博客的用户id</param>
        /// <param name="sort">类型 null：不筛选 -1.全部  0.图片  1.视频   2.头条文章  3.音乐  4.普通文字(表情)</param>
        /// <param name="type">种类null：不筛选 0.公开  1.仅自己可见  2.好友圈  3.粉丝  4.热推  5.广告</param>
        /// <param name="FId">0.默认不处理 1.原创 2.转发</param>
        /// <param name="startTime">开始时间 null：不筛选 </param>
        /// <param name="endTime">结束时间 null：不筛选 </param>
        /// <param name="scope">范围  null：不筛选 1.[主页的博客]=>我自己+朋友圈+我是粉丝的博客 2.[朋友圈的博客]=>只查询朋友圈的博客  3.[特别关注的博客]=>只查询特别关注的博客  4.[自定义分组的博客]=>只查询自定义分组的博客</param>
        /// <param name="grp">分组 null：不筛选 </param>
        /// <returns></returns>
        Task<PagedList<Blog>?> PagedAsync(int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "", Guid? id = null, Guid? wId = null, Guid? uId = null, sbyte? sort = null, sbyte? type = null, BlogForwardType? fId = null, DateTime? startTime = null, DateTime? endTime = null, int? scope = null, string? grp = null, Status? status = null);

        /// <summary>
        /// 获取 blog 的关联信息
        /// </summary>
        /// <param name="blog">需要获取关系信息的 blog</param>
        /// <param name="wId">当前网站的登录用户</param>
        /// <returns></returns>
        Task GetBlogRelation(Blog blog, Guid? wId);

        /// <summary>
        /// 获取当前用户当前年每月的博客数
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        Task<IEnumerable<dynamic>> GetBlogCountGroupbyMonth(Guid uId, int year);

        /// <summary>
        /// 要删除的博客
        /// </summary>
        /// <param name="blog"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> DeleteAsync(Blog blog);

        /// <summary>
        /// 同时转发和评论博客
        /// </summary>
        /// <param name="blog"></param>
        /// <param name="blogComment"></param>
        /// <param name="blogForward"></param>
        /// <returns></returns>
        Task<bool> AddBlogCommentAndForward(Blog blog, BlogComment blogComment, BlogForward blogForward);
    }
}