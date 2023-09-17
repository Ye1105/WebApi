namespace Manager.Server.IServices
{
    public interface IBlogImageLikeService
    {
        /// <summary>
        /// 新增博客图片点赞
        /// </summary>
        /// <param name="iId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> AddAsync(Guid iId, Guid uId);

        /// <summary>
        /// 删除博客图片点赞
        /// </summary>
        /// <param name="iId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> DeleteAsync(Guid iId, Guid uId);

        /// <summary>
        /// 博客图片点赞数
        /// </summary>
        /// <param name="iId"></param>
        /// <returns></returns>
        Task<long?> CountAsync(Guid iId);

        /// <summary>
        /// 博客图片师傅点赞
        /// </summary>
        /// <param name="iId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<bool?> ExsitAsync(Guid iId, Guid uId);
    }
}