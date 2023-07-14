using Manager.Core.Enums;
using Manager.Core.Models.Blogs;
using Manager.Core.Page;
using System.Linq.Expressions;

namespace Manager.Server.IServices
{
    public interface IBlogCommentService
    {
        /// <summary>
        /// 增加博客评论
        /// </summary>
        /// <param name="blogComment"></param>
        /// <returns></returns>
        Task<bool> AddBlogComment(BlogComment blogComment);

        /// <summary>
        /// 返回符合的数据条数
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Task<int> GetCommentCountBy(Expression<Func<BlogComment, bool>> expression, bool isTrack = true);

        /// <summary>
        /// 删除blog
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="grp">分组</param>
        /// <param name="id">评论id</param>
        /// <returns></returns>
        Task<Tuple<bool, string>> DeleteBlogComment(CommentType type, Guid grp, Guid id);

        /// <summary>
        /// 查询博客评论分页列表
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="offset">偏移量</param>
        /// <param name="isTrack">是否跟踪：默认 ture</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        Task<PagedList<BlogComment>> GetPagedList(Expression<Func<BlogComment, bool>> whereLambda, int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "");

        /// <summary>
        /// 查询博客评论分页列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="offset">偏移量</param>
        /// <param name="isTrack">是否跟踪：默认 ture</param>
        /// <param name="orderBy">排序</param>
        /// <param name="id"></param>
        /// <param name="bId">博客id</param>
        /// <param name="uId">发表评论用户id</param>
        /// <param name="type">0.评论   1.【回复】来评论【评论】  2.【回复】来评论【回复】</param>
        /// <param name="pId">分支</param>
        /// <param name="grp">分组 </param>
        /// <param name="startTime">开始时间 null：不筛选</param>
        /// <param name="endTime">结束时间 null：不筛选</param>
        /// <returns></returns>
        Task<PagedList<BlogComment>?> GetPagedList(int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "", Guid? id = null, Guid? bId = null, Guid? uId = null, CommentType[]? type = null, Guid? pId = null, Guid? grp = null, DateTime? startTime = null, DateTime? endTime = null, Status? status = null);
    }
}