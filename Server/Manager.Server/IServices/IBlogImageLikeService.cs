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
        Task<Tuple<bool, string>> AddBlogImageLike(Guid iId, Guid uId);

        /// <summary>
        /// 删除博客图片点赞
        /// </summary>
        /// <param name="iId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> DelBlogImageLike(Guid iId, Guid uId);

        /// <summary>
        /// 博客图片点赞数
        /// </summary>
        /// <param name="iId"></param>
        /// <returns></returns>
        Task<long?> GetBlogImageLikeCountBy(Guid iId);

        /// <summary>
        /// 博客图片师傅点赞
        /// </summary>
        /// <param name="iId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<bool?> GetIsBlogImageLikeByUser(Guid iId, Guid uId);
    }
}