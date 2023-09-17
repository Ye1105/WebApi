using Manager.Core.Enums;
using Manager.Core.Models.Blogs;
using Manager.Core.Page;
using System.Linq.Expressions;

namespace Manager.Server.IServices
{
    public interface IBlogVideoService
    {
        Task<BlogVideo?> FirstOrDefaultAsync(Guid id);

        /// <summary>
        /// 查询博客的视频信息
        /// </summary>
        /// <param name="expression">LINQ</param>
        /// <param name="isTrack">是否跟踪</param>
        /// <returns></returns>
        Task<BlogVideo?> FirstOrDefaultAsync(Expression<Func<BlogVideo, bool>> expression, bool isTrack = true);

        /// <summary>
        /// 查询博客的视频列表
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Task<List<BlogVideo>> QueryAsync(Expression<Func<BlogVideo, bool>> expression, bool isTrack = true);

        /// <summary>
        /// 查询博客视频分页列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="offset"></param>
        /// <param name="isTrack"></param>
        /// <param name="orderBy"></param>
        /// <param name="bId"></param>
        /// <param name="uId"></param>
        /// <param name="title"></param>
        /// <param name="channel"></param>
        /// <param name="collection"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public Task<PagedList<BlogVideo>?> PagedAsync(int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "", Guid? bId = null, Guid? uId = null, string? title = "", string? channel = null, string? collection = null, string? type = null, DateTime? startTime = null, DateTime? endTime = null, Status status = Status.ENABLE);
    }
}