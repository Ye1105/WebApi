using Manager.Core.Models.Blogs;
using Manager.Core.Page;
using System.Linq.Expressions;

namespace Manager.Server.IServices
{
    public interface IBlogImageService
    {
        /// <summary>
        /// 通过id查询博客图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<BlogImage>?> GetBlogImageListById(Guid id);

        /// <summary>
        /// 查询博客的图片信息
        /// </summary>
        /// <param name="expression">LINQ</param>
        /// <param name="isTrack">是否跟踪</param>
        /// <returns></returns>
        Task<List<BlogImage>> GetBlogImageListBy(Expression<Func<BlogImage, bool>> expression, bool isTrack = true);

        /// <summary>
        /// 分页列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="offset"></param>
        /// <param name="isTrack"></param>
        /// <param name="orderBy"></param>
        /// <param name="bId"></param>
        /// <param name="uId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Task<PagedList<BlogImage>?> GetPagedList(int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "", Guid? bId = null, Guid? uId = null, DateTime? startTime = null, DateTime? endTime = null);

        /// <summary>
        /// 更新博客图片
        /// </summary>
        /// <param name="blogImage"></param>
        /// <returns></returns>
        Task<bool> UpdateBlogImage(BlogImage blogImage);
    }
}