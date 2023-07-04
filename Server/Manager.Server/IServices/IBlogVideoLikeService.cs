namespace Manager.Server.IServices
{
    public interface IBlogVideoLikeService
    {
        /// <summary>
        /// 新增博客视频点赞
        /// </summary>
        /// <param name="vId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> AddBlogVideoLike(Guid vId, Guid uId);

        /// <summary>
        /// 删除博客视频点赞
        /// </summary>
        /// <param name="vId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> DelBlogVideoLike(Guid vId, Guid uId);

        /// <summary>
        /// 博客图片点赞数
        /// </summary>
        /// <param name="iId"></param>
        /// <returns></returns>
        Task<long?> GetBlogVideoLikeCountBy(Guid vId);

        /// <summary>
        ///  博客视频点赞数
        /// </summary>
        /// <param name="vId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<bool?> GetIsBlogVideoLikeByUser(Guid vId, Guid uId);
    }
}