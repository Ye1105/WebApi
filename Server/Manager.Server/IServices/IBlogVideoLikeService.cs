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
        Task<Tuple<bool, string>> AddAsync(Guid vId, Guid uId);

        /// <summary>
        /// 删除博客视频点赞
        /// </summary>
        /// <param name="vId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> DeleteAsync(Guid vId, Guid uId);

        /// <summary>
        /// 博客图片点赞数
        /// </summary>
        /// <param name="iId"></param>
        /// <returns></returns>
        Task<long?> CountAsync(Guid vId);

        /// <summary>
        ///  当前博客视频是否被点赞
        /// </summary>
        /// <param name="vId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<bool?> ExsitAsync(Guid vId, Guid uId);
    }
}